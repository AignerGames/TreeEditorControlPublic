using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

using TreeEditorControl.Utility;
using TreeEditorControl.Environment.Implementation;

namespace TreeEditorControl.Example
{
    public class ViewModel : ObservableObject
    {
        private TabViewModel _selectedTab;

        public ViewModel()
        {
            var editorEnvironment = new EditorEnvironment();

            // Disable the undo/redo stack during the node initialization
            editorEnvironment.UndoRedoStack.IsEnabled = false;

            // Create a unique copy of the environment, because all tabs use their own node factory, 
            // but can share the same undo stack
            AddTab(new DataNodes.DataNodeTabViewModel(new EditorEnvironment(editorEnvironment)));
            AddTab(new Default.DefaultTabViewModel(new EditorEnvironment(editorEnvironment)));
            AddTab(new Directory.DirectoryTabViewModel(new EditorEnvironment(editorEnvironment)));
            AddTab(new Dialog.DialogTabViewModel(new EditorEnvironment(editorEnvironment)));

            // Enable the undo/redo stack after the node initialization
            editorEnvironment.UndoRedoStack.IsEnabled = true;

            SelectedTab = TabViewModels.FirstOrDefault();
        }

        public ObservableCollection<TabViewModel> TabViewModels { get; } = new ObservableCollection<TabViewModel>();

        public TabViewModel SelectedTab { get => _selectedTab; set => SetAndNotify(ref _selectedTab, value); }

        public void HandleClosing(CancelEventArgs args)
        {
            foreach (var vm in TabViewModels)
            {
                vm.HandleClosing(args);
            }
        }

        private void AddTab(TabViewModel tabViewModel)
        {
            tabViewModel.TreeChanged += TabViewModel_TreeChanged;

            TabViewModels.Add(tabViewModel);
        }

        private void TabViewModel_TreeChanged(object sender, EventArgs e)
        {
            if(sender is TabViewModel tabViewModel)
            {
                SelectedTab = tabViewModel;
            }
        }
    }

}
