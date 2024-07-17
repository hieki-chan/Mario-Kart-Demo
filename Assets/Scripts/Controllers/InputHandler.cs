using UnityEngine;
using UnityEngine.Events;

namespace KartDemo.Controllers
{
    public abstract class InputHandler : MonoBehaviour
    {
        public abstract Vector2 MoveValue();

        public abstract bool Brake();

        public abstract bool Drift();

        public abstract bool DriftHold();
    }
}