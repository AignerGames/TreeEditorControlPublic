using System;
using TreeEditorControl.Environment;
using TreeEditorControl.Nodes.Implementation;

namespace TreeEditorControl.Example.Default
{
    [Catalog.NodeCatalogInfo("Example container", "Nodes", "")]
    public class DefaultContainer : TreeNodeContainer<DefaultNode>, IDefaultNode
    {
        public DefaultContainer(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {
            Header = $"Container_{DateTime.Now}";
        }
    }
}
