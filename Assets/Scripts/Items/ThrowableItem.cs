using System;
using UnityEngine;

namespace KartDemo.Item
{
    public abstract class ThrowableItem : MonoBehaviour
    {
        public static Pool<Type, ThrowableItem> Pool = new Pool<Type, ThrowableItem>(5);
        [NonEditable] public float spinAngle;
        [NonEditable] public float rotateAngle;


        public virtual void Throw(GameObject thrower, float throwerVelocityZ)
        {

        }
    }
}