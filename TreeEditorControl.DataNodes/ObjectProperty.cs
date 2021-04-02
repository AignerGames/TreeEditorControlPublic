using System;
using System.Reflection;
using TreeEditorControl.Environment;

namespace TreeEditorControl.DataNodes
{
    public class ObjectProperty : NodeProperty
    {
        public ObjectProperty(IEditorEnvironment editorEnvironment, PropertyInfo propertyInfo, string propertyName = null) : base(editorEnvironment, propertyInfo, propertyName)
        {
            DataNode = (DataNode)EditorEnvironment.NodeFactory.CreateNode(propertyInfo.PropertyType);
        }

        public DataNode DataNode { get; }

        public bool IsExpanded { get; set; } = true;

        public override void ReadInstanceValue(object instance)
        {
            var instanceValue = PropertyInfo.GetValue(instance);
            
            if(instanceValue == null)
            {
                instanceValue = Activator.CreateInstance(PropertyInfo.GetType());
            }

            DataNode.SetInstanceValues(instanceValue);
        }

        public override void WriteInstanceValue(object instance)
        {
            var dataInstance = DataNode.GetInstanceValues();

            PropertyInfo.SetValue(instance, dataInstance);

        }
    }
}
