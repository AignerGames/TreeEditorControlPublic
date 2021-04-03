using System;
using TreeEditorControl.Environment;
using TreeEditorControl.Nodes.Implementation;

namespace TreeEditorControl.Example.Default
{
    [Catalog.NodeCatalogInfo("Example node", "Nodes", "")]
    public class DefaultNode : TreeNode, IDefaultNode
    {
        public DefaultNode(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {
            Header = $"Node_{DateTime.Now}";
        }
    }
}
