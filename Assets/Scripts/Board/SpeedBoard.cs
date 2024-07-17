using KartDemo;
using KartDemo.Controllers;
using UnityEngine;

public class SpeedBoard : MonoBehaviour
{
    public float speedForce;
    KartControllerV2 kart;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            if(!kart || kart.GetInstanceID() != other.GetInstanceID())
                kart = other.GetComponent<KartControllerV2>();

            if(kart)
            {
                kart.GetComponent<Rigidbody>().AddForce(kart.transform.forward * speedForce);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            if (!kart || kart.GetInstanceID() != other.GetInstanceID())
                kart = other.GetComponent<KartControllerV2>();

            if (kart)
            {
                kart.GetComponent<Rigidbody>().AddForce(kart.transform.forward * (speedForce* 2));
            }
        }
    }
}

