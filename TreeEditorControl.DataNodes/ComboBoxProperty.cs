﻿using System.Collections.Generic;
using System.Reflection;

using TreeEditorControl.Environment;
using TreeEditorControl.DataNodeAttributes;

namespace TreeEditorControl.DataNodes
{
    public class ComboBoxProperty : NodeProperty
    {
        public ComboBoxProperty(IEditorEnvironment editorEnvironment, IComboBoxValueProvider valueProvider, PropertyInfo propertyInfo, string propertyName = null) 
            : base(editorEnvironment, propertyInfo, propertyName)
        {
            ValueProvider = valueProvider;
        }

        public IComboBoxValueProvider ValueProvider { get; }

        public IReadOnlyList<object> Values => ValueProvider.Values;
    }
}
