//
// Authors:
//   ������ (Johnny Liu) <jingeelio@163.com>
//
// Copyright (c) 2017 ������ (Johnny Liu)
//
// Licensed under the LGPLv3 license. Please see <http://www.gnu.org/licenses/lgpl-3.0.html> for license text.
//

using System;
using System.IO;
using JointCode.AddIns.Core.Helpers;
using JointCode.AddIns.Metadata.Assets;

namespace JointCode.AddIns.Core.FileScanning
{
    abstract class FileScanner
    {
        protected internal const string ThisAssemblyName = "JointCode.AddIns.dll";

        internal abstract ScanFilePack ScanForFilePack(string probingDirectory, string addinDirectory, string manifestFile,
            AddinFilePack matchingFilePack);

        protected internal static bool IsAssembly(string file)
        {
            return file.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)
                || file.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    class XmlBasedFileScanner : FileScanner
    {
        internal override ScanFilePack ScanForFilePack(string probingDirectory, string addinDirectory, string manifestFileName,
            AddinFilePack matchingFilePack)
        {
            //var addinDirectoryName = Path.GetFileName(addinDirectory);
            var filePack = new ScanFilePack { AddinDirectory = addinDirectory, AddinProbingDirectory = probingDirectory };
            return matchingFilePack != null
                ? DoScanForFilePack(filePack, addinDirectory, manifestFileName, matchingFilePack)
                : DoScanForFilePack(filePack, addinDirectory, manifestFileName);
        }

        static ScanFilePack DoScanForFilePack(ScanFilePack filePack, string addinDirectory, string manifestFileName, AddinFilePack matchingFilePack)
        {
            //λ�ڲ���ļ���������ļ�������ΪǱ�ڰ���������õ��嵥�ļ�������ļ�
            //��������Щ�ļ������ǲ�������ļ��������ļ��������ļ���Ҫ�����ļ���������
            var files = Directory.GetFiles(addinDirectory);

            var shouldRescan = false;
            foreach (var file in files)
            {
                if (IsAssembly(file))
                {
                    shouldRescan |= IsScannableAssemblyFile(file, matchingFilePack);
                    filePack.AddAssemblyFile(file);
                }
                else if (filePack.ManifestFile == null && file.EndsWith(manifestFileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    shouldRescan |= IsScannableManifestFile(file, matchingFilePack);
                    filePack.ManifestFile = file;
                }
                else
                {
                    filePack.AddDataFile(file);
                }
            }

            if (filePack.ManifestFile == null) // ����嵥�ļ�����λ�ڲ����Ŀ¼�£�����ڲ����Ŀ¼��δ�ҵ��嵥�ļ�������Ϊ�ⲻ��һ�����
                return null;

            // �ݹ�ɨ������Ŀ¼�µ�������Ŀ¼�Բ��ҳ��򼯺������ļ�
            var subdirectories = Directory.GetDirectories(addinDirectory);
            foreach (var subdirectory in subdirectories)
                shouldRescan |= ScanSubdirecotires(filePack, subdirectory, matchingFilePack);

            return shouldRescan ? filePack : null;
        }

        // �ݹ�ɨ������Ŀ¼�µ�������Ŀ¼�Բ��ҳ��򼯺������ļ�
        static bool ScanSubdirecotires(ScanFilePack filePack, string subdirectory, AddinFilePack matchingFilePack)
        {
            var shouldRescan = false;
            var files = Directory.GetFiles(subdirectory);
            
            foreach (var file in files)
            {
                if (IsAssembly(file))
                {
                    shouldRescan |= IsScannableAssemblyFile(file, matchingFilePack);
                    filePack.AddAssemblyFile(file);
                }
                else
                {
                    filePack.AddDataFile(file);
                }
            }

            var childDirectories = Directory.GetDirectories(subdirectory);
            foreach (var childDirectory in childDirectories)
                shouldRescan |= ScanSubdirecotires(filePack, childDirectory, matchingFilePack);

            return shouldRescan;
        }

        static ScanFilePack DoScanForFilePack(ScanFilePack filePack, string addinDirectory, string manifestFileName)
        {
            //λ�ڲ���ļ���������ļ�������ΪǱ�ڰ���������õ��嵥�ļ�������ļ�
            //��������Щ�ļ������ǲ�������ļ��������ļ��������ļ���Ҫ�����ļ���������
            var files = Directory.GetFiles(addinDirectory);

            foreach (var file in files)
            {
                if (IsAssembly(file))
                    filePack.AddAssemblyFile(file);
                else if (filePack.ManifestFile == null && file.EndsWith(manifestFileName, StringComparison.InvariantCultureIgnoreCase))
                    filePack.ManifestFile = file;
                else
                    filePack.AddDataFile(file);
            }

            if (filePack.ManifestFile == null)
                return null; // ����嵥�ļ�����λ�ڲ����Ŀ¼�£�����ڲ����Ŀ¼��δ�ҵ��嵥�ļ�������Ϊ�ⲻ��һ�����

            // �ݹ�ɨ������Ŀ¼�µ�������Ŀ¼�Բ��ҳ��򼯺������ļ�
            var subdirectories = Directory.GetDirectories(addinDirectory);
            foreach (var subdirectory in subdirectories)
                ScanSubdirecotires(filePack, subdirectory);

            return filePack;
        }

        // �ݹ�ɨ������Ŀ¼�µ�������Ŀ¼�Բ��ҳ��򼯺������ļ�
        static void ScanSubdirecotires(ScanFilePack filePack, string subdirectory)
        {
            var files = Directory.GetFiles(subdirectory);

            foreach (var file in files)
            {
                if (IsAssembly(file))
                    filePack.AddAssemblyFile(file);
                else
                    filePack.AddDataFile(file);
            }

            var childDirectories = Directory.GetDirectories(subdirectory);
            foreach (var childDirectory in childDirectories)
                ScanSubdirecotires(filePack, childDirectory);
        }

        static bool IsScannableAssemblyFile(string file, AddinFilePack matchingFilePack)
        {
            if (matchingFilePack.AssemblyFiles == null)
                return true;
            foreach (var assemblyFile in matchingFilePack.AssemblyFiles)
            {
                if (!file.EndsWith(assemblyFile.FilePath, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (IoHelper.GetLastWriteTime(file) != assemblyFile.LastWriteTime)
                    return true;
            }
            return false;
        }

        static bool IsScannableManifestFile(string file, AddinFilePack matchingFilePack)
        {
            var fi = IoHelper.GetFileInfo(file);
            return fi.Length != matchingFilePack.ManifestFile.FileLength 
                || fi.LastWriteTime != matchingFilePack.ManifestFile.LastWriteTime 
                || IoHelper.GetFileHash(file) != matchingFilePack.ManifestFile.FileHash;
        }
    }
}
