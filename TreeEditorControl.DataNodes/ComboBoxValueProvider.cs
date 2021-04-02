using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TreeEditorControl.DataNodeAttributes;

namespace TreeEditorControl.DataNodes
{
    public class ComboBoxValueProvider : IComboBoxValueProvider
    {
        private readonly Func<IReadOnlyList<object>> _valueProvider;

        public ComboBoxValueProvider(Func<IReadOnlyList<object>> valueProvider)
        {
            _valueProvider = valueProvider;
        }

        public IReadOnlyList<object> Values => _valueProvider();
    }
}
