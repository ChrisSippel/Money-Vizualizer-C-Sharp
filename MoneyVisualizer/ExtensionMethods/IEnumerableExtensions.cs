using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyVisualizer.ExtensionMethods
{
    public static class IEnumerableExtensions
    {
        // Copied from: https://stackoverflow.com/questions/1509442/linq-style-for-each
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
