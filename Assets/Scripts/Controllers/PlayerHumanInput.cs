using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

namespace KartDemo.Controllers
{
    public class PlayerHumanInput : InputHandler
    {
        KartInput kartInput;

        [Header("Camera")]
        public CinemachineVirtualCamera frontCam;

        public UnityEvent OnThrowItem;
        public UnityEvent OnRespawn;


        private void Awake()
        {
            kartInput = new KartInput();
        }

        private void OnEnable()
        {
            kartInput.Enable();
        }

        private void OnDisable()
        {
            kartInput.Disable();
        }

        private void Update()
        {
            //switch camera
            if (kartInput.Player.SwitchCam.WasPressedThisFrame())
            {
                frontCam.gameObject.SetActive(!frontCam.gameObject.activeInHierarchy);
            }

            if(kartInput.Player.ThrowIItem.WasPressedThisFrame())
            {
                OnThrowItem?.Invoke();
            }

            if (kartInput.Player.Respawn.WasPressedThisFrame())
            {
                OnRespawn?.Invoke();
            }
        }

        public override Vector2 MoveValue()
        {
            return kartInput.Player.Move.ReadValue<Vector2>();
        }

        public override bool Brake()
        {
            return kartInput.Player.Brake.IsPressed();
        }

        public override bool Drift()
        {
            return kartInput.Player.Drift.WasPressedThisFrame();
        }

        public override bool DriftHold()
        {
            return kartInput.Player.Drift.IsPressed();
        }
    }
}