using System;

namespace TreeEditorControl.DataNodeAttributes
{
    public class NodeHeaderFormatAttribute : Attribute
    {
        public NodeHeaderFormatAttribute(string headerFormat)
        {
            HeaderFormat = headerFormat;
        }

        public string HeaderFormat { get; }
    }
}
