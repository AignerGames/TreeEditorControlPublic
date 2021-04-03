using System.Reflection;

using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Utility;
using TreeEditorControl.ViewModel;
using TreeEditorControl.Environment.Implementation;

namespace TreeEditorControl.Example.Default
{
    public class DefaultTabViewModel : TabViewModel
    {
        public DefaultTabViewModel(EditorEnvironment editorEnvironment) : base("Default", editorEnvironment)
        {
            editorEnvironment.NodeFactory = new TreeNodeFactory(editorEnvironment);

            EditorViewModel = new TreeEditorViewModel(editorEnvironment);

            EditorViewModel.AddDefaultCommands();
            EditorViewModel.AddDefaultContextMenuCommands();

            EditorViewModel.CatalogItems.AddItems(NodeCatalogItem.CreateItemsForAssignableTypes(typeof(IDefaultNode), Assembly.GetExecutingAssembly()));

            var containerNode = editorEnvironment.NodeFactory.CreateNode<DefaultContainer>();

            EditorViewModel.AddRootNode(containerNode);
        }
    }
}
