namespace JointCode.AddIns.Extension
{
    public class ExtensionPointDescription
    {
        public bool Loaded { get; internal set; }

        // Guid �����ڲ�ͬӦ�ó���֮��Ψһ�ر�ʶһ�� Addin�����ɲ�ͬ Addin �ṩ������ ExtensionPoint ��һ��Ӧ�ó����б���Ψһ��
        /// <summary>
        /// Gets the name of <see cref="IExtensionPoint"/> that uniquely identify an <see cref="IExtensionPoint"/> within an application.
        /// </summary>
        public string Name { get; internal set; }

        public string Description { get; internal set; }

        // The type name of the IExtensionPoint/IExtensionBuilder
        public string TypeName { get; internal set; }
    }
}