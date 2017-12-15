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
        internal abstract FilePack GetFilePack(string probeDirectory, string addinDirectory, string manifestFile,
            AddinFilePack addinFilePack);

        protected internal static bool IsAssembly(string file)
        {
            return file.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)
                || file.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    class XmlBasedFileScanner : FileScanner
    {
        internal override FilePack GetFilePack(string probeDirectory, string addinDirectory, string manifestFile,
            AddinFilePack addinFilePack)
        {
            var addinDirectoryName = Path.GetFileName(addinDirectory);
            var filePack = new FilePack { AddinDirectoryName = addinDirectoryName, AddinProbeDirectory = probeDirectory };

            //λ�ڲ���ļ���������ļ�������ΪǱ�ڰ���������õ��嵥�ļ�������ļ�
            //��������Щ�ļ������ǲ�������ļ��������ļ��������ļ���Ҫ���ļ�����������
            var files = Directory.GetFiles(addinDirectory);

            if (addinFilePack != null)
            {
                var shouldRescan = false;
                foreach (var file in files)
                {
                    if (IsAssembly(file))
                    {
                        shouldRescan |= IsScannableAssemblyFile(file, addinFilePack);
                        filePack.AddAssemblyFile(file);
                    }
                    else if (filePack.ManifestFile == null && file.EndsWith(manifestFile, StringComparison.InvariantCultureIgnoreCase))
                    {
                        shouldRescan |= IsScannableManifestFile(file, addinFilePack);
                        filePack.ManifestFile = file;
                    }
                    else
                    {
                        filePack.AddDataFile(file);
                    }
                }
                if (!shouldRescan)
                    return null;
            }
            else
            {
                foreach (var file in files)
                {
                    if (IsAssembly(file))
                        filePack.AddAssemblyFile(file);
                    else if (filePack.ManifestFile == null && file.EndsWith(manifestFile, StringComparison.InvariantCultureIgnoreCase))
                        filePack.ManifestFile = file;
                    else
                        filePack.AddDataFile(file);
                }
            }

            //λ�ڲ���ļ��е����ļ����е��ļ�������Ϊ�����ļ�
            var dataDirectories = Directory.GetDirectories(addinDirectory);
            foreach (var dataDirectory in dataDirectories)
                GetAllDataFiles(dataDirectory, filePack);

            return filePack.ManifestFile == null ? null : filePack;
        }

        static void GetAllDataFiles(string dataDirectory, FilePack filePack)
        {
            //Logger.Info(string.Format("Scanning directory [{0}]...", directory));
            var dataFiles = Directory.GetFiles(dataDirectory);
            foreach (var dataFile in dataFiles) 
            	filePack.AddDataFile(dataFile);
            foreach (string subDir in Directory.GetDirectories(dataDirectory))
                GetAllDataFiles(subDir, filePack);
        }

        static bool IsScannableAssemblyFile(string file, AddinFilePack addinFilePack)
        {
            if (addinFilePack.AssemblyFiles == null)
                return true;
            foreach (var assemblyFile in addinFilePack.AssemblyFiles)
            {
                if (!file.EndsWith(assemblyFile.FilePath, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (IoHelper.GetLastWriteTime(file) != assemblyFile.LastWriteTime)
                    return true;
            }
            return false;
        }

        static bool IsScannableManifestFile(string file, AddinFilePack addinFilePack)
        {
            return IoHelper.GetLastWriteTime(file) != addinFilePack.ManifestFile.LastWriteTime 
                || IoHelper.GetFileHash(file) != addinFilePack.ManifestFile.FileHash;
        }
    }
}
