using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Utils
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Traverse a hierarchical structure and flatten it (for easier LINQ usage)
        /// </summary>
        /// <typeparam name="T">Type of the objects in hierarchy</typeparam>
        /// <param name="source">Source of the hierarchy</param>
        /// <param name="childrenSelector">How to select children that will be flatten</param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            // Cycle through all of the items.
            foreach (T item in source)
            {
                // Yield the item.
                yield return item;

                // Yield all of the children.
                if (childrenSelector?.Invoke(item) != null)
                {
                    foreach (T child in childrenSelector(item).Flatten(childrenSelector))
                    {
                        // Yield the item.
                        yield return child;
                    }
                }
            }
        }
    }
}
