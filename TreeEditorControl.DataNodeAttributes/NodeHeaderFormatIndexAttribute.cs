using System;

namespace TreeEditorControl.DataNodeAttributes
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
