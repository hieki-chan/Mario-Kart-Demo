using KartDemo.Controllers;
using UnityEngine;

namespace KartDemo.Item
{
    public abstract class WorldItem : MonoBehaviour, IActiveHandler
    {
        public static Pool<System.Type, WorldItem> Pool = new Pool<System.Type, WorldItem>(5);

        [SerializeField] private float reactivationTime = 5;
        [SerializeField] private float spinSpeed = 5;
        [SerializeField] private Vector3 spinAngle;
        float timer;

        public bool reActive = true;

        [SerializeField] private AudioClip collectedClip;

        float IActiveHandler.reactivationTime => reactivationTime;
        float IActiveHandler.timer { get => timer; set => timer = value; }

        private void Start()
        {
            if(spinSpeed != 0)
                LeanTween.rotateAroundLocal(gameObject, spinAngle, -360, spinSpeed == 0 ? 0 : 1 / spinSpeed).setLoopType(LeanTweenType.linear);
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerConfig.PLAYER_TAG))
            {
                if (other.TryGetComponent<KartControllerV2>(out var player))
                {
                    OnPlayerTrigger(player, player.GetComponent<PlayerTrack>());

                    if (reActive)
                        Reactivating();
                    else
                        Pool.Return(GetType(), this);

                    if (!collectedClip)
                        return;
                    player.m_PlayerSounds.EffectOneShot(collectedClip);
                }
            }
        }

        protected virtual void OnPlayerTrigger(KartControllerV2 player, PlayerTrack track)
        {
            
        }

        protected virtual void Reactivating()
        {
            Activator.Active(this);
        }

        public void OnInactive() => gameObject.SetActive(false);

        public void OnActive() => gameObject.SetActive(true);
    }
}