using System.Collections.Generic;
using UnityEngine;

namespace KartDemo.Utils
{
    public static class ListExtensions
    {
        public static void RemoveAtSwapBack<T>(this List<T> list, int index)
        {
            if (list == null || index < 0 || index > list.Count)
            {
                return;
            }

            int count = list.Count;
            list[index] = list[count - 1];
            list.RemoveAt(count - 1);
        }

        public static T PickOne<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;

            return list[Random.Range(0, list.Count)];
        }
    }
}