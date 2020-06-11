using System;
using System.Collections.Generic;

namespace TreeEditorControl.UndoRedo.Implementation
{
    public class UndoRedoListWrapper<T>
    {
        private readonly IUndoRedoStack _undoRedoStack;

        public UndoRedoListWrapper(IUndoRedoStack undoRedoStack, IList<T> list)
        {
            _undoRedoStack = undoRedoStack;
            List = list;
        }

        public IList<T> List { get; }

        public int Count => List.Count;

        public T this[int index] => List[index];
            
        public void Add(T item)
        {
            Insert(List.Count, item);
        }

        public void Insert(int index, T item)
        {
            PushCommand(() => InsertRedoAction(index, item), () => InsertUndoAction(index));
        }

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void Remove(T item)
        {
            var index = IndexOf(item);
            if(index < 0)
            {
                return;
            }

            RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            var item = List[index];
            PushCommand(() => RemoveAtRedoAction(index), () => RemoveAtUndoAction(index, item));
        }

        public void Clear()
        {
            var items = new List<T>(List);

            PushCommand(() => ClearRedoAction(), () => ClearUndoAction(items));
        }

        protected virtual void InsertRedoAction(int index, T item)
        {
            List.Insert(index, item);
        }

        protected virtual void InsertUndoAction(int index)
        {
            List.RemoveAt(index);
        }

        protected virtual void RemoveAtRedoAction(int index)
        {
            List.RemoveAt(index);
        }

        protected virtual void RemoveAtUndoAction(int index, T item)
        {
            List.Insert(index, item);
        }

        protected virtual void ClearRedoAction()
        {
            List.Clear();
        }

        protected virtual void ClearUndoAction(IReadOnlyList<T> items)
        {
            foreach(var item in items)
            {
                List.Add(item);
            }
        }

        private void PushCommand(Action redoAction, Action undoAction)
        {
            _undoRedoStack.ExecuteAndPush(new UndoRedoCommand(redoAction, undoAction));
        }
    }
}
