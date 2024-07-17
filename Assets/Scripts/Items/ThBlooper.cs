using KartDemo.Controllers;
using KartDemo.Utils;
using UnityEngine;
using System.Collections;
using KartDemo.Item;
using KartDemo;
using System;

public sealed class ThBlooper : ThrowableItem
{
    public SlowModifier slowModifier;
    public float speed;
    float currentSpeed;

    public override void Throw(GameObject thrower, float throwerVelocityZ)
    {
        KartControllerV2 player = RaceManager.instance.Players.PickOne();
        while (player.gameObject == thrower && RaceManager.instance.PlayerCount > 1)
            player = RaceManager.instance.Players.PickOne();

        StartCoroutine(Throwing(player));
        currentSpeed = 0;
    }

    IEnumerator Throwing(KartControllerV2 player)
    {
        while (true)
        {
            Vector3 playerPosUp = player.transform.position + player.transform.up * 5;
            currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime / 3.5f);
            transform.position = Vector3.MoveTowards(transform.position, playerPosUp, Time.deltaTime * currentSpeed);

            Vector3 toPlayer = -(transform.position - playerPosUp).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toPlayer), Time.deltaTime * 5);
            if ((transform.position - playerPosUp).sqrMagnitude <= 1)
            {
                slowModifier.player = player.transform;
                player.AddModifier(slowModifier);
                Pool.Return(GetType(), this);
                yield break;
            }

            Debug.DrawLine(transform.position, playerPosUp, Color.red);

            yield return null;
        }
    }

    [Serializable]
    public class SlowModifier : Modifier
    {
        static SimplePool<Transform> SlowEffectPool = new SimplePool<Transform>();
        public float duration; 
        public Transform slowEffectPrefab;
        Transform slowEffect;
        [HideInInspector]
        public Transform player;

        public override void OnApplied()
        {
            base.OnApplied();
            slowEffect = SlowEffectPool.GetOrCreate(slowEffectPrefab, Vector3.zero, Quaternion.identity);
            slowEffect.transform.parent = player;
            slowEffect.localPosition = Vector3.zero;
            slowEffect.localRotation = Quaternion.Euler(90, 0, 0);
        }

        public override void OnRemoved()
        {
            SlowEffectPool.Return(slowEffect);
        }

        protected override float Duration()
        {
            return duration;
        }

        public override float ModifySpeed(float speed)
        {
            return speed * .6f;
        }
    }
}