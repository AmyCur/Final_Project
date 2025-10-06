using UnityEngine;
using SoundManagement;

namespace SoundManagement{
    public enum Sound{
        HUM,
        LIGHTNING_ATTACK,
        WALKING
    }
}


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour{
    [SerializeField] AudioClip[] soundList;
    static SoundManager instance;
    AudioSource audioSource;

    void Awake() => instance = this;    
    void Start() => audioSource = GetComponent<AudioSource>();

    public static void StopSound(){
        instance.audioSource.Stop();
    }

    public static void PlaySound(Sound sound, float volume=1){

        // Limit max volume
        volume = volume > 5 ? Mathf.Clamp(volume, 1, 5) : volume;
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound]);
    }
}