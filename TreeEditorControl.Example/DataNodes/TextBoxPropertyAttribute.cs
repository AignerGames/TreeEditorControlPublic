using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example.DataNodes
{
    public class TextBoxPropertyAttribute : NodePropertyAttribute
    {
        public TextBoxPropertyAttribute(bool multiline = false, string propertyName = null) : base(propertyName)
        {
            Multiline = multiline;
        }

        public bool Multiline { get; }
    }
}
