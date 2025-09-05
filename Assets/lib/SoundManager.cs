
/*using System.Collections;
using MathsAndSome;
using UnityEngine;

namespace SoundManager
{
    namespace Sound
    {
        public static class Play
        {
            public static IEnumerator PlaySound(AudioSource source, AudioClip clip)
            {
                if (source == null)
                {
                    source = new();
                }

                source.PlayOneShot(clip, 1);

                yield return new WaitForSeconds(clip.length);

                GameObject.Destroy(source);
            }
        }
        
    }
}*/
