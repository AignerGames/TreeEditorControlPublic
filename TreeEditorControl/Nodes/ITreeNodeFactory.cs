using System;
using TreeEditorControl.Catalog;

namespace TreeEditorControl.Nodes
{
    public interface ITreeNodeFactory
    {
        ITreeNode CreateNode(NodeCatalogItem catalogItem);

        ITreeNode CreateNode(Type nodeType);
    }
}
