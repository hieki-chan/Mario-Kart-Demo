using UnityEngine;

namespace KartDemo.Utils
{
    public static class Rotation
    {
        public static Vector3 LerpAngle(Vector3 from, Vector3 to, float t)
        {
            Vector3 angle = new Vector3(
                angle.x = Mathf.LerpAngle(from.x, to.x, t),
                angle.y = Mathf.LerpAngle(from.y, to.y, t),
                angle.z = Mathf.LerpAngle(from.z, to.z, t)
                );
            return angle;
        }
    }
}