using KartDemo.Item;
using KartDemo.Utils;
using System.Collections;
using UnityEngine;

public sealed class ThBananaPeel : ThrowableItem
{
    public float thowForce;
    public float thowForceUp;
    public float gravityForce;
    public Rigidbody rb;
    public Collider col;
    public WBananaPeel WBananaPeel;

    private void OnEnable()
    {
        col.isTrigger = true;
    }

    public override void Throw(GameObject thrower, float throwerVelocityZ)
    {
        transform.position = Position.Offset(thrower.transform, new Vector3(0, 2, 3));
        col.isTrigger = false;
        rb.AddForce(thrower.transform.forward * ((thowForce + throwerVelocityZ) * Time.deltaTime * 100), ForceMode.Impulse);
        rb.AddRelativeForce(Vector3.up * (thowForceUp * Time.deltaTime * 100), ForceMode.Impulse);
        StartCoroutine(Gravity());
    }

    readonly WaitForFixedUpdate wait = new WaitForFixedUpdate();
    private IEnumerator Gravity()
    {
        while (true)
        {
            rb.AddRelativeForce(Vector3.down * (gravityForce * Time.fixedDeltaTime * 100), ForceMode.Acceleration);
            yield return wait;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            WorldItem.Pool.GetOrCreate(WBananaPeel.GetType(), WBananaPeel, transform.position - transform.up * .5f, transform.rotation);
            Pool.Return(this.GetType(), this);
        }
    }
}