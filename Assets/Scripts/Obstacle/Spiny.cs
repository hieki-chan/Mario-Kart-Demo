using KartDemo.Controllers;
using UnityEngine;
using KartDemo;
using System.Collections.Generic;

public class Spiny : MonoBehaviour
{
    //public Collider m_Collider;
    public List<Transform> movementPath;
    public Vector3 offset;
    public float moveSpeed;
    public float rotateSpeed;
    int currentPoint;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            KartControllerV2 player = collision.gameObject.GetComponent<KartControllerV2>();

            if (!player)
                return;

            //Vector3 hitPoint = m_Collider.ClosestPoint(collision.transform.position);
            player.OnObstacleHit(transform.position);
        }
    }

    private void Update()
    {
        if (movementPath.Count == 0)
            return;

        Vector3 nextPoint = movementPath[currentPoint].position + offset;
        transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, nextPoint, Time.deltaTime * moveSpeed),
            Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((nextPoint - transform.position).normalized, movementPath[currentPoint].up), Time.deltaTime * rotateSpeed));
        if((transform.position - nextPoint).sqrMagnitude <= .15f * .15f)
        {
            currentPoint++;
            if(currentPoint >= movementPath.Count)
            {
                currentPoint = 0;
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (movementPath.Count == 0)
            return;

        Gizmos.color = Color.green;
        int i = 0;
        for (; i < movementPath.Count - 1; i++)
        {
            Gizmos.DrawLine(movementPath[i].position, movementPath[i + 1].position);
            Gizmos.DrawWireSphere(movementPath[i].position, 1f);
        }

        Gizmos.DrawWireSphere(movementPath[i].position, 1f);
        Gizmos.DrawLine(movementPath[i].position, movementPath[0].position);
    }
#endif
}

