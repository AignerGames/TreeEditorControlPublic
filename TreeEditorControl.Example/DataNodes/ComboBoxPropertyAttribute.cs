using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example.DataNodes
{
    public class ComboBoxPropertyAttribute : NodePropertyAttribute
    {
        public ComboBoxPropertyAttribute(string valueProviderId, string propertyName = null) : base(propertyName)
        {
            ValueProviderId = valueProviderId;
        }

        public string ValueProviderId { get; }
    }
}
