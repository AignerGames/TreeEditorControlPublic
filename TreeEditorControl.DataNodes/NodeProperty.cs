using System;
using System.Reflection;


using TreeEditorControl.UndoRedo.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.Utility;

namespace TreeEditorControl.DataNodes
{
    public abstract class NodeProperty : EditorObject
    {
        private UndoRedoValueWrapper<object> _valueWrapper;

        protected NodeProperty(IEditorEnvironment editorEnvironment, PropertyInfo propertyInfo, string propertyName = null) : base(editorEnvironment)
        {
            _valueWrapper = CreateUndoRedoWrapper<object>(nameof(Value));

            Name = propertyName ?? propertyInfo.Name;
            PropertyInfo = propertyInfo;            
        }

        public string Name { get; }

        public object Value
        {
            get => _valueWrapper.Value;
            set
            {
                try
                {
                    var convertedValue = PropertyInfo.PropertyType.IsEnum && value is string stringValue
                        ? Enum.Parse(PropertyInfo.PropertyType, stringValue)
                        : Convert.ChangeType(value, PropertyInfo.PropertyType);

                    _valueWrapper.Value = convertedValue;
                }
                catch
                {
                    // Ignore the invalid input
                }
            }
        }

        public PropertyInfo PropertyInfo { get; }

        public virtual void ReadInstanceValue(object instance)
        {
            Value = PropertyInfo.GetValue(instance);
        }

        public virtual void WriteInstanceValue(object instance)
        {
            PropertyInfo.SetValue(instance, Value);
        }
    }
}
