using System;
using System.Collections.Generic;
using System.Linq;

namespace acme_bank.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Sets the value on the current IEnumerable collection of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="updateMethod"></param>
        /// <returns></returns>
        public static IEnumerable<T> SetValue<T>(this IEnumerable<T> items, Action<T>
             updateMethod)
        {
            foreach (T item in items)
            {
                updateMethod(item);
            }
            return items;
        }
    }
}
