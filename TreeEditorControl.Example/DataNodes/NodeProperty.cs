﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


using TreeEditorControl.UndoRedo.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.Utility;

namespace TreeEditorControl.Example.DataNodes
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
                    var convertedValue = Convert.ChangeType(value, PropertyInfo.PropertyType);

                    _valueWrapper.Value = convertedValue;
                }
                catch
                {
                    // Ignore the invalid input
                }
            }
        }

        public PropertyInfo PropertyInfo { get; }

        public void ReadInstanceValue(object instance)
        {
            Value = PropertyInfo.GetValue(instance);
        }

        public void WriteInstanceValue(object instance)
        {
            PropertyInfo.SetValue(instance, Value);
        }
    }
}
