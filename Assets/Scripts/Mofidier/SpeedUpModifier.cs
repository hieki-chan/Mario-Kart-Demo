using UnityEngine;

namespace KartDemo
{
    public class SpeedUpModifier : Modifier
    {
        private readonly float duration;
        private readonly float speed;
        private float speedLerp;

        public SpeedUpModifier(float duration, float speed)
        {
            this.duration = duration;
            this.speed = speed;
            speedLerp = 0;
        }

        public override float ModifySpeed(float speed)
        {
            speedLerp = Mathf.Lerp(speedLerp, this.speed, Time.deltaTime * 3);
            return speed + speedLerp;
        }

        protected override float Duration()
        {
            return duration;
        }

        public override bool CanStackUp()
        {
            return false;
        }

        public override void OnReset()
        {
            elapsed = 0;
        }
    }
}