namespace TreeEditorControl.ViewModel
{
    public interface IContextMenuOpeningHandler
    {
        /// <summary>
        /// Used to update the context menu in the view model.
        /// Returns true if the context menu should be shown
        /// </summary>
        bool HandleContextMenuOpening();
    }
}
