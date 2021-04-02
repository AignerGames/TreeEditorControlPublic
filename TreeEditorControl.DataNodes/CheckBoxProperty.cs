using System.Reflection;
using TreeEditorControl.Environment;

namespace TreeEditorControl.DataNodes
{
    public class CheckBoxProperty : NodeProperty
    {
        public CheckBoxProperty(IEditorEnvironment editorEnvironment, PropertyInfo propertyInfo, string propertyName = null) : base(editorEnvironment, propertyInfo, propertyName)
        {
        }
    }
}
