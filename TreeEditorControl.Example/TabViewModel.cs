using System;
using System.ComponentModel;

using TreeEditorControl.Utility;
using TreeEditorControl.ViewModel;
using TreeEditorControl.Environment.Implementation;

namespace TreeEditorControl.Example
{
    public abstract class TabViewModel : ObservableObject
    {
        private TreeEditorViewModel _editorViewModel;

        public TabViewModel(string header, EditorEnvironment editorEnvironment)
        {
            Header = header;
            EditorEnvironment = editorEnvironment;
        }

        public event EventHandler TreeChanged;

        public string Header { get; }

        public TreeEditorViewModel EditorViewModel
        {
            get => _editorViewModel;
            protected set
            {
                if(_editorViewModel != null)
                {
                    _editorViewModel.NodeChanged -= EditorViewModel_NodeChanged;
                }

                _editorViewModel = value;

                if (_editorViewModel != null)
                {
                    _editorViewModel.NodeChanged += EditorViewModel_NodeChanged;
                }
            }
        }

        public virtual void HandleClosing(CancelEventArgs args)
        {

        }

        protected EditorEnvironment EditorEnvironment { get; }

        private void EditorViewModel_NodeChanged(object sender, Nodes.NodeChangedArgs e)
        {
            TreeChanged?.Invoke(this, e);
        }
    }
}
