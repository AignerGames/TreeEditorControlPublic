
using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.Example.Dialog.Actions;

namespace TreeEditorControl.Example.Dialog
{
    /// <summary>
    /// Wrapper around the default <see cref="TreeNodeFactory"/> with some additional logic.
    /// </summary>
    internal class CustomNodeFactory : ITreeNodeFactory
    {
        private readonly IEditorEnvironment _editorEnvironment;
        private readonly TreeNodeFactory _defaultNodeFactory;

        public CustomNodeFactory(IEditorEnvironment editorEnvironment)
        {
            _editorEnvironment = editorEnvironment;
            _defaultNodeFactory = new TreeNodeFactory(_editorEnvironment);
        }

        public ITreeNode CreateNode(NodeCatalogItem catalogItem)
        {
            if(catalogItem.Name == DialogTabViewModel.ShowTextHelloWorldCatalogName)
            {
                return CreateShowTextHelloWorld();
            }

            return _defaultNodeFactory.CreateNode(catalogItem);
        }

        public ShowTextAction CreateShowTextHelloWorld()
        {
            return new ShowTextAction(_editorEnvironment, "Hello world!");
        }

        public DialogRootNode CreateDialogRootNode() => _defaultNodeFactory.CreateNode<DialogRootNode>();
    }
}
