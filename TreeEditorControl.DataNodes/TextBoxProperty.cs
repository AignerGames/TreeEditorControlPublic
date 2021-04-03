using System.Reflection;
using TreeEditorControl.Environment;

namespace TreeEditorControl.DataNodes
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
