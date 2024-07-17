using KartDemo;
using KartDemo.Controllers;
using System;
using UnityEngine;

public class JumpBoard : MonoBehaviour
{
    public float jumpForce;
    KartControllerV2 kart;

    [SerializeField]
    JumpModifier jumpModifier;

    [Serializable]
    public class JumpModifier : Modifier
    {
        [SerializeField]
        private float duration;
        [SerializeField]
        private float speed;
        private float speedLerp;

        [HideInInspector]
        public KartControllerV2 player;

        public override float ModifySpeed(float speed)
        {
            if(!player.IsGrouned)
                speedLerp = Mathf.Lerp(speedLerp, this.speed, Time.deltaTime * 4);
            else
                speedLerp = Mathf.Lerp(speedLerp, 0, Time.deltaTime / 2);

            return speed + speedLerp;
        }

        protected override float Duration()
        {
            return duration;
        }

        public override void OnReset()
        {
            elapsed = 0;
        }

        public override bool IsExpired()
        {
            bool expired = base.IsExpired();
            //reset value
            if(expired)
            {
                speedLerp = 0;
                elapsed = 0;
            }
            return expired;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            if(!kart || kart.GetInstanceID() != other.GetInstanceID())
                kart = other.GetComponent<KartControllerV2>();

            if(kart)
            {
                // kart.MultiplySpeed(jumpForce);
                jumpModifier.player = kart;
                kart.AddModifier(jumpModifier);
                kart.m_KartEffects.Stop(kart.m_KartEffects.boostBurst);
                kart.m_KartEffects.Play(kart.m_KartEffects.boostBurst);
            }
        }
    }

/*    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            if (!kart || kart.GetInstanceID() != other.GetInstanceID())
                kart = other.GetComponent<KartControllerV2>();

            if (kart)
            {
                kart.GetComponent<Rigidbody>().AddForce(kart.transform.forward * jumpForce);
            }
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            if (!kart || kart.GetInstanceID() != other.GetInstanceID())
                kart = other.GetComponent<KartControllerV2>();

            if (kart)
            {
                kart.GetComponent<Rigidbody>().AddForce(kart.transform.up * jumpForce);
                kart.JumpSpin();
            }
        }
    }
}

