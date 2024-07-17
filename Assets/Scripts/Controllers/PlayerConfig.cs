using UnityEngine;

namespace KartDemo
{
    public static class PlayerConfig
    {
        public const string PLAYER_TAG = "Player";
        public static readonly int IdleHash = Animator.StringToHash("Idle");
        public static readonly int StartTurboHash = Animator.StringToHash("StartTurbo");
        public static readonly int DriftHopHash = Animator.StringToHash("DriftHop");
        public static readonly int SpinRightHash = Animator.StringToHash("SpinRight");
        public static readonly int SpinLeftHash = Animator.StringToHash("SpinLeft");
        public static readonly int JumpHash = Animator.StringToHash("Jump");
        public static readonly int ObstacleHitHash = Animator.StringToHash("ObstacleHit");
        public static readonly int LightHitLeftHash = Animator.StringToHash("LightHitLeft");
        public static readonly int LightHitRightHash = Animator.StringToHash("LightHitRight");
        public static readonly int IdleMoveParamHash = Animator.StringToHash("IdleMove");
    }
}