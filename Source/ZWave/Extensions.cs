using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave
{
    public static class Extensions
    {
#if WINDOWS_UWP
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T element)
        {
            yield return element;
            foreach (var item in source)
            {
                yield return item;
            }
        }
#endif
    }
}
