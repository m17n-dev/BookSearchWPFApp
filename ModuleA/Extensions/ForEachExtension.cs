using System;
using System.Collections.Generic;

namespace ModuleA.Extensions {
    public static class ForEachExtension {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            if (enumerable != null) {
                foreach (var cur in enumerable) {
                    action(cur);
                }
            }
        }
    }
}
