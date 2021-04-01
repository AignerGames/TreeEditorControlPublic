using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TreeEditorControl.Environment;

namespace TreeEditorControl.Example.DataNodes
{
    public class TextBoxProperty : NodeProperty
    {
        public TextBoxProperty(IEditorEnvironment editorEnvironment, PropertyInfo propertyInfo, bool multiline = false, string propertyName = null) 
            : base(editorEnvironment, propertyInfo, propertyName)
        {
            Multiline = multiline;
        }

        public bool Multiline { get; }
    }
}
