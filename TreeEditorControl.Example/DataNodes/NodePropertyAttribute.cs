using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example.DataNodes
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
