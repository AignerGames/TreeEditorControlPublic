using System.Reflection;

using TreeEditorControl.Catalog;
using TreeEditorControl.Utility;
using TreeEditorControl.ViewModel;
using TreeEditorControl.Environment.Implementation;
using TreeEditorControl.Commands;
using TreeEditorControl.Example.Data;
using System.Linq;
using TreeEditorControl.Nodes;
using System.Collections.ObjectModel;
using System.Windows;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogTabViewModel : TabViewModel
    {
        const string EditorDataPath = "Data.json";
        const string GameExportPath = "GameExport.json";

        public const string ShowTextHelloWorldCatalogName = "ShowText HelloWorld";

        private readonly FileLoadHandler _fileLoadHandler;
        private readonly FileSaveHandler _fileSaveHandler;

        public DialogTabViewModel(EditorEnvironment editorEnvironment) : base("Dialog", editorEnvironment)
        {
            _fileLoadHandler = new FileLoadHandler(editorEnvironment);
            _fileSaveHandler = new FileSaveHandler();


            var nodeFactory = new CustomNodeFactory(editorEnvironment);

            EditorViewModel = new TreeEditorViewModel(editorEnvironment, nodeFactory);

            EditorViewModel.AddDefaultCommands();
            EditorViewModel.AddDefaultContextMenuCommands();

            EditorViewModel.CatalogItems.AddItems(NodeCatalogItem.CreateItemsForAssignableTypes(typeof(DialogNode), Assembly.GetExecutingAssembly()));

            //EditorViewModel.CatalogItems.Add(new NodeCatalogItem(ShowTextHelloWorldCatalogName, "Actions", "ShowText with 'Hello world!'", typeof(ShowTextAction)));

            //EditorViewModel.ContextMenuCommands.Add(new Commands.ContextMenuCommand("Say 'Hello world!'",
            //    () => EditorViewModel.SelectedNode is ShowTextAction, 
            //    () => (EditorViewModel.SelectedNode as ShowTextAction).Text = "Hello world!"));

            var dialogRootNode = nodeFactory.CreateDialogRootNode();

            EditorViewModel.AddRootNode(dialogRootNode);



            // TODO: Toolbar menu
            EditorViewModel.ContextMenuCommands.Add(ContextMenuCommand.Seperator);
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Expand node", ExpandNodeFull, () => EditorViewModel.SelectedNode != null));
            EditorViewModel.ContextMenuCommands.Add(ContextMenuCommand.Seperator);
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Insert dialog root", AddDialogRoot));
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Delete dialog root", DeleteDialogRoot, () => EditorViewModel.SelectedNode is DialogRootNode));
            EditorViewModel.ContextMenuCommands.Add(ContextMenuCommand.Seperator);
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Load", LoadFile));
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Save", SaveFile));
            EditorViewModel.ContextMenuCommands.Add(ContextMenuCommand.Seperator);
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Reset", ResetFile));
        }


        public ObservableCollection<StringViewModel> Actors { get; } = new ObservableCollection<StringViewModel>();

        public ObservableCollection<StringViewModel> Variables { get; } = new ObservableCollection<StringViewModel>();

        private void LoadFile()
        {
            if(MessageBox.Show("Load file?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            EditorEnvironment.UndoRedoStack.IsEnabled = false;
            EditorEnvironment.UndoRedoStack.Reset();

            Actors.Clear();
            Variables.Clear();
            EditorViewModel.ClearRootNodes();

            _fileLoadHandler.Load(EditorDataPath, this);

            EditorViewModel.RootNodes.FirstOrDefault()?.ExpandRecursive();

            EditorEnvironment.UndoRedoStack.IsEnabled = true;
        }

        private void SaveFile()
        {
            if(MessageBox.Show("Save file?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            EditorEnvironment.UndoRedoStack.IsEnabled = false;

            _fileSaveHandler.Save(EditorDataPath, GameExportPath, this);

            EditorEnvironment.UndoRedoStack.IsEnabled = true;
        }

        private void AddDialogRoot()
        {
            var insertIndex = -1;

            if(EditorViewModel.SelectedNode is DialogRootNode selectedRootNode)
            {
                insertIndex = EditorViewModel.RootNodes.IndexOf(selectedRootNode) + 1;
            }

            var dialogRootNode = new DialogRootNode(EditorEnvironment, "NewDialog");

            EditorViewModel.AddRootNode(dialogRootNode, insertIndex);
        }

        private void DeleteDialogRoot()
        {
            if (!(EditorViewModel.SelectedNode is DialogRootNode rootNode))
            {
                return;
            }

            if (MessageBox.Show($"Delete {rootNode.Header}?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            rootNode.IsSelected = false;
            EditorViewModel.RemoveRootNode(rootNode);
        }

        private void ExpandNodeFull()
        {
            EditorViewModel.SelectedNode?.ExpandRecursive();
        }

        private void ResetFile()
        {
            if(MessageBox.Show("Reset file?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            EditorEnvironment.UndoRedoStack.IsEnabled = false;
            EditorEnvironment.UndoRedoStack.Reset();

            Actors.Clear();
            Variables.Clear();
            EditorViewModel.ClearRootNodes();

            AddDialogRoot();

            EditorEnvironment.UndoRedoStack.IsEnabled = true;
        }
    }
}
