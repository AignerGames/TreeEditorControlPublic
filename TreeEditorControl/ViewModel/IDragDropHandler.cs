namespace TreeEditorControl.ViewModel
{
    public interface IDragDropHandler<TDrag, TDrop> where TDrag : class where TDrop : class
    {
        bool CanDrag(TDrag drag);

        bool CanDrop(TDrag drag, TDrop drop);

        void Drop(TDrag drag, TDrop drop);
    }
}
