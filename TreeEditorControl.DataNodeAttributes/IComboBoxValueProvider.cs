using System.Collections.Generic;

namespace TreeEditorControl.DataNodeAttributes
{
    public interface IComboBoxValueProvider
    {
        IReadOnlyList<object> Values { get; }
    }
}
