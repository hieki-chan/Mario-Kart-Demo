using KartDemo.Item;
using KartDemo.Controllers;
using System.Collections;
using UnityEngine;
using KartDemo;
using KartDemo.Utils;
using Unity.VisualScripting;

public sealed class ThGreenShell : ThrowableItem
{
    const float LIFE_TIME = 10;
    [NonEditable]
    public float lifeTime;
    public float radius;


    public float thowForce;
    public float thowForceUp;
    public float gravityForce;
    public Rigidbody rb;
    public Collider col;

    private void OnEnable()
    {
        col.isTrigger = true;
        transform.localScale = Vector3.one;
        lifeTime = 0;
    }

    public override void Throw(GameObject thrower, float throwerVelocityZ)
    {
        Vector3 toPlayer = DetectPlayer(thrower.transform);
        transform.position = Position.Offset(thrower.transform, new Vector3(0, 2, 3));
        transform.rotation = Quaternion.LookRotation(toPlayer);
        col.isTrigger = false;
        rb.AddForce(thrower.transform.forward * ((thowForce + throwerVelocityZ * 1.5f) * Time.deltaTime * 150), ForceMode.Impulse);
        //rb.AddRelativeForce(Vector3.up * (thowForceUp * Time.deltaTime * 100), ForceMode.Impulse);
        StartCoroutine(Move());
    }

    Vector3 DetectPlayer(Transform thrower)
    {
        Transform closest = null;
        float closetDistance = Mathf.Infinity;

        foreach (var player in RaceManager.instance.Players)
        {
            if (player == thrower)
                continue;

            float distance = (transform.position - player.transform.position).sqrMagnitude;

            if (distance < closetDistance)
            {
                closest = player.transform;
                closetDistance = distance;
            }
        }
        closetDistance = Mathf.Sqrt(closetDistance);


        if (closetDistance <= radius)
        {
            return (closest.position - transform.position).normalized;
        }

        return thrower.forward;
    }

    readonly WaitForFixedUpdate wait = new WaitForFixedUpdate();
    private IEnumerator Move()
    {
        while (true)
        {
            //ground normal rotattion
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 10, 1 << 6))
            {
                transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation, Time.deltaTime * 9f);
            }
            //gravity
            rb.AddRelativeForce((Vector3.down + Vector3.forward) * (gravityForce * Time.fixedDeltaTime * 200), ForceMode.Acceleration);

            //size
            transform.localScale = Vector3.Lerp(transform.lossyScale, new Vector3(2.5f, 2.5f, 2.5f), Time.deltaTime);

            //life time
            lifeTime += Time.deltaTime;
            if (lifeTime >= LIFE_TIME)
            {
                Pool.Return(GetType(), this);
                yield break;
            }

            //move forward
            //rb.AddForce(transform.forward * Time.deltaTime * 1000, ForceMode.Force);
            yield return wait;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {

        }
        else
        {
            transform.rotation = Quaternion.Inverse(transform.rotation);
            rb.AddRelativeForce(Vector3.forward * (thowForce * Time.deltaTime * 50), ForceMode.Impulse);
        }

        if (collision.gameObject.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            if (collision.gameObject.TryGetComponent<KartControllerV2>(out var controller))
            {
                controller.OnObstacleHit(transform.position, 0);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}