using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KartDemo.Utils;
using System.Collections.Generic;
using KartDemo.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace KartDemo.Controllers
{
    public class KartEffects : MonoBehaviour
    {
        readonly SimplePool<TrailRenderer> pool = new SimplePool<TrailRenderer>();

        public TargetedEffect driftLeftEffectPrefab;
        public TargetedEffect driftRightEffectPrefab;

        [Space, Header("Speed Boost")]
        public TargetedEffect speedBoost01;
        public TargetedEffect speedBoost02;
        public TargetedEffect speedBoost03;
        public TargetedEffect speedBoost04;

        public CustomPostProcessing speedLines;
        public PostProcessVolume speedVolume;

        public TargetedEffect boostBurst;

        [Space, Header("SKidMarks")]
        public TrailRenderer skidMarksPrefab;


        public TargetedEffect skidRearLeft;
        public TargetedEffect skidRearRight;
        public TargetedEffect skidFrontLeft;
        public TargetedEffect skidFrontRight;

        private void Start()
        {
            GetEffects().Foreach(e => InitEffect(e));

            pool.Create(skidMarksPrefab, 0);
        }

        private IEnumerable<TargetedEffect> GetEffects()
        {
            return GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(t => t.FieldType == typeof(TargetedEffect) || t.FieldType.IsSubclassOf(typeof(TargetedEffect)))
                .Select(f => (TargetedEffect)f.GetValue(this));
        }

        public void Play(TargetedEffect effect)
        {
            effect.instance.SetActive(true);
            effect.instance.transform.position = Position.Offset(effect.target, effect.offset);
        }

        public void Stop(TargetedEffect effect)
        {
            effect.instance.SetActive(false);
        }

        public void PlaySkidMarks()
        {
            if (skidRearLeft.instance || skidFrontRight.instance)
            {
                return;
            }

            var rearLeft = pool.Get();
            var rearRight = pool.Get();
            rearLeft.transform.parent = skidRearLeft.target;
            rearRight.transform.parent = skidFrontRight.target;

            skidRearLeft.instance = rearLeft.gameObject;
            skidFrontRight.instance = rearRight.gameObject;

            Play(skidRearLeft);
            Play(skidFrontRight);

            pool.Return(rearLeft, true);
            pool.Return(rearRight, true);
        }

        public void StopSkidMarks()
        {
            if (skidRearLeft.instance)
            {
                skidRearLeft.instance.transform.parent = null;
                skidRearLeft.instance = null;
            }
            if (skidFrontRight.instance)
            {
                skidFrontRight.instance.transform.parent = null;
                skidFrontRight.instance = null;
            }
        }

        public void PlaySpeedUp()
        {
            Play(speedBoost01);
            Play(speedBoost02);
            Play(speedBoost03);
            Play(speedBoost04);

            if(speedLines)
                speedLines.enabled = true;

            if(speedVolume)
                speedVolume.gameObject.SetActive(true);
        }

        public void StopSpeedUp()
        {
            Stop(speedBoost01);
            Stop(speedBoost02);
            Stop(speedBoost03);
            Stop(speedBoost04);

            if(speedLines)
                speedLines.enabled = false;
            if (speedVolume)
                speedVolume.gameObject.SetActive(false);
        }

        private void InitEffect(TargetedEffect effect)
        {
            if (effect.prefab == null)
                return;

            GameObject go = Instantiate(effect.prefab);
            go.SetActive(false);
            if (effect.parent)
                go.transform.parent = effect.target;
            effect.instance = go;
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            GetEffects().Foreach(e =>
            {
                if (e.target)
                    Gizmos.DrawWireSphere(Position.Offset(e.target, e.offset), .1f);
            });
        }
#endif
    }

    [Serializable]
    public class TargetedEffect
    {
        public GameObject prefab;
        [NonSerialized]
        public GameObject instance;
        public Transform target;
        public Vector3 offset;
        public bool parent;
    }
}