using System;

namespace TreeEditorControl.Catalog
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeCatalogInfoAttribute : Attribute
    {
        public NodeCatalogInfoAttribute(string name, string category, string description)
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
