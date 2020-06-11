using System;
using System.Collections.Generic;

namespace TreeEditorControl.UndoRedo.Implementation
{
    public class UndoRedoValueWrapper<T>
    {
        private readonly IUndoRedoStack _undoRedoStack;
        private T _value;
        private readonly string _propertyName;
        private readonly Action<string> _notifyAction;

        public UndoRedoValueWrapper(IUndoRedoStack undoRedoStack, T defaultValue = default, string propertyName = null, Action<string> notifyAction = null)
        {
            _undoRedoStack = undoRedoStack;
            _value = defaultValue;
            _propertyName = propertyName;
            _notifyAction = notifyAction;
        }

        public UndoRedoValueWrapper(IUndoRedoStack undoRedoStack, string propertyName = null, Action<string> notifyAction = null) : 
            this(undoRedoStack, default, propertyName, notifyAction)
        {

        }

        public T Value 
        { 
            get => _value;
            set => UpdateValue(value);
        }

        private void UpdateValue(T newValue)
        {
            var oldValue = _value;

            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return;
            }

            var command = new UndoRedoCommand(() => SetValueCommandAction(newValue), () => SetValueCommandAction(oldValue));
            _undoRedoStack.ExecuteAndPush(command);
        }

        protected virtual void SetValueCommandAction(T newValue)
        {
            _value = newValue;

            _notifyAction?.Invoke(_propertyName);
        }

        public override string ToString() => $"{GetType().Name} {_propertyName} {_value}";
    }
}
