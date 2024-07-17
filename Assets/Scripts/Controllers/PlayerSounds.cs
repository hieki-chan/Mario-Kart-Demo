using UnityEngine;
using KartDemo.Utils;

namespace KartDemo.Controllers
{
    public class PlayerSounds : MonoBehaviour
    {
        public AudioClip stopIdleSound;
        public AudioClip accelNormal;
        public AudioClip DriftHopSound;
        public AudioClip DriftSteerSound;
        public AudioClip kartBumpSound;
        public AudioClip[] BoostSounds;

        [Header("Audio Sources")]
        public AudioSource KartAudioSource;
        public AudioSource CharacterAudioSource;
        public AudioSource EffectAudioSource;

        //CHARACTER SOURCE
        public void PlayBoostSound()
            => CharacterAudioSource.PlayOnce(BoostSounds.PickOne());

        //KART SOURCE
        public void KartLoop(AudioClip clip, bool ignoreOnPlaying = true)
            => KartAudioSource.PlayLoop(clip, ignoreOnPlaying);

        public void KartOneShot(AudioClip clip)
            => KartAudioSource.PlayOneShot(clip);

        //EFFECT SOURCE
        public void EffectLoop(AudioClip clip, bool ignoreOnPlaying = true)
            => EffectAudioSource.PlayLoop(clip, ignoreOnPlaying);
        public void EffectOneShot(AudioClip clip)
            => EffectAudioSource.PlayOneShot(clip);

        public void EffectStopIf(AudioClip clip)
            => EffectAudioSource.StopIf(clip);


    }
}
