using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TreeEditorControl.Example.Data
{
    [Serializable]
    public class EditorData
    {
        public List<string> Variables { get; set; } = new List<string>();
    }
}
