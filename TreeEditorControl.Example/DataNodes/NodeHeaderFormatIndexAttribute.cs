using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example.DataNodes
{
    public class NodeHeaderFormatIndexAttribute : Attribute
    {
        public NodeHeaderFormatIndexAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; }
    }
}
