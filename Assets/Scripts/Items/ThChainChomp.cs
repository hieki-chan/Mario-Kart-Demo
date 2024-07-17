using KartDemo;
using KartDemo.Controllers;
using KartDemo.Item;
using KartDemo.Utils;
using System.Collections;
using UnityEngine;

public sealed class ThChainChomp : ThrowableItem
{
    static SimplePool<Transform> explosionPool = new SimplePool<Transform>();

    public float duration;
    public float explosionRadius;
    public float speed;
    //float currentSpeed;
    [SerializeField]
    ChainChopModifier modifier;
    public GameObject explosionEffect;
    Collider[] cols = new Collider[18];

    public override void Throw(GameObject thrower, float throwerVelocityZ)
    {
        KartControllerV2 player = RaceManager.instance.Players.PickOne();
        while (player.gameObject == thrower && RaceManager.instance.PlayerCount > 1)
            player = RaceManager.instance.Players.PickOne();
        StartCoroutine(Throwing(player));
        //currentSpeed = 0;
    }

    IEnumerator Throwing(KartControllerV2 player)
    {
        while (true)
        {
            Vector3 playerPosUp = player.transform.position + player.transform.up;
            //currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime / 3.5f);
            transform.position = Vector3.MoveTowards(transform.position, playerPosUp, Time.deltaTime * speed);

            Vector3 toPlayer = -(transform.position - playerPosUp).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toPlayer), Time.deltaTime * 10);

            if ((transform.position - playerPosUp).sqrMagnitude <= 1)
            {
                OnExplosion(player);
                yield break;
            }

            Debug.DrawLine(transform.position, playerPosUp, Color.red);

            yield return null;
        }
    }

    private void OnExplosion(KartControllerV2 player)
    {
        int playerCount = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, cols);
        var explosionEffectInstance = explosionPool.GetOrCreate(explosionEffect.transform, transform.position, transform.rotation).gameObject;
        Activator.Disable(explosionEffectInstance, 3, (e) => explosionPool.Return(e.transform));

        for (int i = 0; i < playerCount; i++)
        {
            if (cols[i].TryGetComponent(out KartControllerV2 kart))
            {
                kart.SpinHit(Random.Range(0, 2) == 0 ? 1 : -1);
                kart.OnObstacleHit(transform.position, 0);
                player.AddModifier(modifier);
            }
        }

        Pool.Return(GetType(), this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    [global::System.Serializable]
    class ChainChopModifier : Modifier
    {
        [SerializeField]
        float duration;

        public override float ModifySpeed(float speed)
        {
            return 0;
        }

        protected override float Duration()
        {
            return duration;
        }
    }
}

