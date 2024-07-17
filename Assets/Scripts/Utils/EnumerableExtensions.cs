using System;
using System.Collections.Generic;
using UnityEngine;

namespace KartDemo.Utils
{
    public static class EnumerableExtensions
    {
        public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T> callback)
        {
            foreach (var item in enumerable)
            {
                callback?.Invoke(item);
            }
        }

        /// <summary>
        /// Finds the position closest to the given one.
        /// </summary>
        /// <param name="position">World position.</param>
        /// <param name="otherPositions">Other world positions.</param>
        /// <returns>Closest position.</returns>
        public static T FindClosest<T, T2>(this IEnumerable<T> otherPositions, T2 position, out float closetDistance) where T : Component where T2 : Component
        {
            T closest = null;
            closetDistance = Mathf.Infinity;

            foreach (var otherPosition in otherPositions)
            {
                float distance = (position.transform.position - otherPosition.transform.position).sqrMagnitude;

                if (distance < closetDistance)
                {
                    closest = otherPosition;
                    closetDistance = distance;
                }
            }
            closetDistance = Mathf.Sqrt(closetDistance);
            return closest;
        }

        public static int FindClosestIndex<T, T2>(this IEnumerable<T> otherPositions, T2 position, out float closetDistance) where T : Component where T2 : Component
        {
            int closestIndex = -1;
            int i = -1;
            closetDistance = Mathf.Infinity;

            foreach (var otherPosition in otherPositions)
            {
                i++;
                float distance = (position.transform.position - otherPosition.transform.position).sqrMagnitude;

                if (distance < closetDistance)
                {
                    closestIndex = i;
                    closetDistance = distance;
                }
            }
            closetDistance = Mathf.Sqrt(closetDistance);
            return closestIndex;
        }
    }
}