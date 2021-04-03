using System;

namespace TreeEditorControl.DataNodeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class NodePropertyAttribute : Attribute
    {
        public NodePropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
