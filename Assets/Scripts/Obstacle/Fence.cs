using KartDemo.Controllers;
using UnityEngine;
using KartDemo;

public class Fence : MonoBehaviour
{
    public Collider m_Collider;
    KartControllerV2 kart;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            if (!kart || collision.gameObject.GetInstanceID() != kart.GetInstanceID())
            {
                if (!collision.gameObject.TryGetComponent(out kart))
                {
                    return;
                }
            }

            Vector3 hitPoint = m_Collider.ClosestPoint(collision.transform.position);
            kart.OnObstacleHit(hitPoint);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            if(!kart || collision.gameObject.GetInstanceID() != kart.GetInstanceID())
            {
                if(!collision.gameObject.TryGetComponent(out kart))
                {
                    return;
                }
            }

            Vector3 hitPoint = m_Collider.ClosestPoint(collision.transform.position);
            kart.OnObstacleHit(hitPoint, .975f);
        }
    }
}

