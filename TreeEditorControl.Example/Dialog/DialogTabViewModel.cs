using System.Reflection;

using TreeEditorControl.Catalog;
using TreeEditorControl.Utility;
using TreeEditorControl.ViewModel;
using TreeEditorControl.Environment.Implementation;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogTabViewModel : TabViewModel
    {
        public const string ShowTextHelloWorldCatalogName = "ShowText HelloWorld";

        public DialogTabViewModel(EditorEnvironment editorEnvironment) : base("Dialog", editorEnvironment)
        {
            var nodeFactory = new CustomNodeFactory(editorEnvironment);

            EditorViewModel = new TreeEditorViewModel(editorEnvironment, nodeFactory);

            EditorViewModel.AddDefaultCommands();
            EditorViewModel.AddDefaultContextMenuCommands();

            EditorViewModel.CatalogItems.AddItems(NodeCatalogItem.CreateItemsForAssignableTypes(typeof(IDialogNode), Assembly.GetExecutingAssembly()));

            EditorViewModel.CatalogItems.Add(new NodeCatalogItem(ShowTextHelloWorldCatalogName, "Actions", "ShowText with 'Hello world!'", typeof(ShowTextAction)));

            EditorViewModel.ContextMenuCommands.Add(new Commands.ContextMenuCommand("Say 'Hello world!'",
                () => EditorViewModel.SelectedNode is ShowTextAction, 
                () => (EditorViewModel.SelectedNode as ShowTextAction).Text = "Hello world!"));

            var containerNode = nodeFactory.CreateDialogNode();

            EditorViewModel.AddRootNode(containerNode);
        }
    }
}
