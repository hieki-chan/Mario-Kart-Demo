using UnityEngine;

namespace KartDemo
{
    public class Modifier
    {
        protected float elapsed;

        public virtual void OnApplied()
        {
            elapsed = 0;
        }

        public virtual void Update(float deltaTime)
        {
            elapsed += Time.deltaTime;
        }

        public virtual float ModifySpeed(float speed)
        {
            return speed;
        }

        public virtual bool CanStackUp()
        {
            return false;
        }

        /// <summary>
        /// resets the value if the object cannot stack up
        /// </summary>
        public virtual void OnReset()
        {

        }

        protected virtual float Duration()
        {
            return 0;
        }

        public virtual bool IsExpired()
        {
            return elapsed >= Duration();
        }

        public virtual void OnRemoved()
        {

        }
    }
}