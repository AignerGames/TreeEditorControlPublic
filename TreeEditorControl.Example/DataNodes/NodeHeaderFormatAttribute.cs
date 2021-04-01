using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example.DataNodes
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
