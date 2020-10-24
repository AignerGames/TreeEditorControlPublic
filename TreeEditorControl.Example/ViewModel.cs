using System;
using System.Collections.ObjectModel;

using TreeEditorControl.Utility;
using TreeEditorControl.Environment.Implementation;
using System.Linq;
using System.ComponentModel;

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

            //AddTab(new Default.DefaultTabViewModel(editorEnvironment));
            //AddTab(new Directory.DirectoryTabViewModel(editorEnvironment));
            AddTab(new Dialog.DialogTabViewModel(editorEnvironment));

            // Enable the undo/redo stack after the node initialization
            editorEnvironment.UndoRedoStack.IsEnabled = true;

            SelectedTab = TabViewModels.FirstOrDefault();
        }

        public ObservableCollection<TabViewModel> TabViewModels { get; } = new ObservableCollection<TabViewModel>();

        public TabViewModel SelectedTab { get => _selectedTab; set => SetAndNotify(ref _selectedTab, value); }

        public void HandleClosing(CancelEventArgs args)
        {
            foreach(var vm in TabViewModels)
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
