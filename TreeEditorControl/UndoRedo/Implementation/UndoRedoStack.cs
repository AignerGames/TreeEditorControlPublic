using System.Collections.Generic;

namespace TreeEditorControl.UndoRedo.Implementation
{
    public class UndoRedoStack : IUndoRedoStack
    {
        /// <summary>
        /// This is used as a stack but has to be implemented as a list to support removing at the start
        /// (Commands have to be removed from the start when the max stack size is reached)
        /// </summary>
        private readonly LinkedList<IUndoRedoCommand> _undoStack = new LinkedList<IUndoRedoCommand>();

        private readonly Stack<IUndoRedoCommand> _redoStack = new Stack<IUndoRedoCommand>();

        private readonly List<IUndoRedoCommand> _sequenceCommandBuffer = new List<IUndoRedoCommand>();

        private readonly int _maxUndoStackSize;
        
        private uint _sequenceId;
        private uint _sequenceStartId;

        private bool _isSequenceActive;

        public UndoRedoStack(int maxUndoStackSize = 30)
        {
            _maxUndoStackSize = maxUndoStackSize;
        }

        public bool IsEnabled { get; set; } = true;

        public bool CanUndo => IsEnabled && !_isSequenceActive && _undoStack.Count > 0;

        public bool CanRedo => IsEnabled &&  !_isSequenceActive && _redoStack.Count > 0;

        public void Push(IUndoRedoCommand command)
        {
            if(!IsEnabled)
            {
                return;
            }

            if(_isSequenceActive)
            {
                _sequenceCommandBuffer.Add(command);
            }
            else
            {
                _redoStack.Clear();

                if(_undoStack.Count >= _maxUndoStackSize)
                {
                    _undoStack.RemoveFirst();
                }

                _undoStack.AddLast(command);
            }
        }

        public uint StartSequence()
        {
            UpdateSequenceId();

            if (!_isSequenceActive)
            {
                _sequenceStartId = _sequenceId;

                _isSequenceActive = true;
            }

            return _sequenceId;
        }

        public bool EndSequence(uint sequenceId)
        {
            if(!_isSequenceActive || _sequenceStartId != sequenceId)
            {
                return false;
            }

            // Set to false before the push call otherwise the multi command would be pushed to the sequence buffer
            _isSequenceActive = false;

            if (_sequenceCommandBuffer.Count > 0)
            {
                Push(new UndoRedoMultiCommand(_sequenceCommandBuffer));

                _sequenceCommandBuffer.Clear();
            }

            return true;
        }

        public void Undo()
        {
            if(!CanUndo)
            {
                return;
            }

            var command = _undoStack.Last.Value;
            _undoStack.RemoveLast();

            command.Undo();
            _redoStack.Push(command);
        }

        public void Redo()
        {
            if(!CanRedo)
            {
                return;
            }

            var command = _redoStack.Pop();
            command.Redo();
            _undoStack.AddLast(command);

        }

        public void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            _sequenceCommandBuffer.Clear();

            _isSequenceActive = false;

            // The sequence id's don't have to be reset here, because they wrap anyways
        }

        private void UpdateSequenceId()
        {
            if(_sequenceId == uint.MaxValue)
            {
                _sequenceId = 0;
            }

            _sequenceId++;
        }
    }
}
