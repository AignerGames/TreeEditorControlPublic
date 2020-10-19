using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeEditorControl.Catalog;

namespace TreeEditorControl.Nodes
{
    public interface IInitializeFromCatalogItem
    {
        void Initialize(NodeCatalogItem catalogItem);
    }
}
