using System;

namespace TreeEditorControl.Catalog
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeCatalogInfoAttribute : Attribute
    {
        public NodeCatalogInfoAttribute(string name = null, string category = null, string description = null)
        {
            Name = name;
            Category = category;
            Description = description;
        }

        public string Name { get; }

        public string Category { get; }

        public string Description { get; }
    }
}
