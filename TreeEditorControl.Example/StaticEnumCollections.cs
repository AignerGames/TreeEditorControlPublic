using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example
{
    public static class StaticEnumCollections
    {

        private static ReadOnlyObservableCollection<T> Create<T>() where T : Enum
        {
            var collection = new ObservableCollection<T>(Enum.GetValues(typeof(T)).OfType<T>());

            return new ReadOnlyObservableCollection<T>(collection);
        }
    }
}
