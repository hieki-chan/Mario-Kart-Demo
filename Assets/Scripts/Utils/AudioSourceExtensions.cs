using UnityEngine;

namespace KartDemo.Utils
{
    public static class AudioSourceExtensions
    {
        public static void PlayOnce(this AudioSource source, AudioClip clip, bool ignoreOnPlaying = true)
        {
            if (ignoreOnPlaying && IsPlaying(source, clip))
                return;
            source.clip = clip;
            source.loop = false;
            source.Play();
        }

        public static void PlayLoop(this AudioSource source, AudioClip clip, bool ignoreOnPlaying = true)
        {
            if (ignoreOnPlaying && IsPlaying(source, clip))
                return;
            source.clip = clip;
            source.loop = true;
            source.Play();
        }

        public static void StopIf(this AudioSource source, AudioClip clip)
        {
            if (IsPlaying(source, clip))
                source.Stop();
        }

        public static bool IsPlaying(this AudioSource source, AudioClip clip)
        {
            return source.isPlaying && source.clip == clip;
        }
    }
}