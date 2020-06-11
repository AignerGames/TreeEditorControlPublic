using System.Collections.Generic;

namespace TreeEditorControl.Utility
{
    public static class CollectionExtensions
    {
        public static void AddItems<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach(var item in items)
            {
                list.Add(item);
            }
        }
    }
}
