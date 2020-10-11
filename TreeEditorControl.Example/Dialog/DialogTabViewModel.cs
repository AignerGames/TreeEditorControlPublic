using System.Reflection;

using TreeEditorControl.Catalog;
using TreeEditorControl.Utility;
using TreeEditorControl.ViewModel;
using TreeEditorControl.Environment.Implementation;
using TreeEditorControl.Commands;
using TreeEditorControl.Example.Data;
using System.Linq;
using TreeEditorControl.Nodes;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogTabViewModel : TabViewModel
    {
        const string FilePath = "Data.json";

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
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Add dialog root", AddDialogRoot));
            EditorViewModel.ContextMenuCommands.Add(ContextMenuCommand.Seperator);
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Load", LoadFile));
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Save", SaveFile));
        }

        private void LoadFile()
        {
            EditorEnvironment.UndoRedoStack.IsEnabled = false;
            EditorEnvironment.UndoRedoStack.Reset();

            EditorViewModel.ClearRootNodes();

            var loadedNodes = _fileLoadHandler.Load(FilePath);

            EditorViewModel.AddRootNodes(loadedNodes);

            EditorEnvironment.UndoRedoStack.IsEnabled = true;
        }

        private void SaveFile()
        {
            EditorEnvironment.UndoRedoStack.IsEnabled = false;

            _fileSaveHandler.Save(FilePath, EditorViewModel.RootNodes.OfType<DialogRootNode>());

            EditorEnvironment.UndoRedoStack.IsEnabled = true;
        }

        private void AddDialogRoot()
        {
            var dialogRootNode = new DialogRootNode(EditorEnvironment, "NewDialog");

            EditorViewModel.AddRootNode(dialogRootNode);
        }

        private void ExpandNodeFull()
        {
            if(EditorViewModel.SelectedNode != null)
            {
                ExpandRecursive(EditorViewModel.SelectedNode);
            }

            void ExpandRecursive(ITreeNode node)
            {
                if(node is IReadableNodeContainer container)
                {
                    container.IsExpanded = true;

                    foreach(var child in container.Nodes)
                    {
                        ExpandRecursive(child);
                    }
                }
            }
        }
    }
}
