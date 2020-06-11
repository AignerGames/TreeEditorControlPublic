namespace TreeEditorControl.UndoRedo
{
    public interface IUndoRedoStack
    {
        /// <summary>
        /// If set to false: Push commands will be ignored and undo/redo isn't possible.
        /// Can be used to disable the stack during initial node setup, for example setting properties
        /// after a file reload shouldn't store undo commands. 
        /// </summary>
        bool IsEnabled { get; set; }

        bool CanUndo { get; }

        bool CanRedo { get; }

        void Push(IUndoRedoCommand command);

        /// <summary>
        /// Starts a sequence for a multi command.
        /// Call <see cref="EndSequence(uint)"/> with the returned sequence id.
        /// 
        /// Every <see cref="StartSequence"/> call returns a new id, but the stack will store the 
        /// "sequence start id" internally.
        /// The <see cref="EndSequence(uint)"/> call will do nothing if the given id doesn't match
        /// the current "sequence start id".
        /// This can be used for nested start/end sequence calls. 
        /// </summary>
        uint StartSequence();

        /// <summary>
        /// End the current sequence if the id matches with the current "sequence start id"
        /// See <see cref="StartSequence"/>.
        /// (The current "sequence start id" has to be updated after a successful <see cref="EndSequence(uint)"/> call)
        /// </summary>
        /// <param name="sequenceId"></param>
        bool EndSequence(uint sequenceId);

        void Undo();

        void Redo();

        void Reset();

    }
}
