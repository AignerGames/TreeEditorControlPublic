using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example.DataNodes
{
    public interface IComboBoxValueProvider
    {
        IReadOnlyList<object> Values { get; }
    }
}
