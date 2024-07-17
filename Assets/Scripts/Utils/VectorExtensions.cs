using UnityEngine;

namespace KartDemo.Utils
{
    public static class VectorExtensions
    {
        public static Vector3 ToFlat(this Vector3 source)
        {
            return new Vector3(source.x, 0, source.z);
        }
    }
}