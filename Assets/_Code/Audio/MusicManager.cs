using UnityEngine;

namespace Cur.Audio{

	[System.Serializable]
	public class Audio{
		public AudioClip clip;
	}
	// Im too depressed for ts
	public static class MusicManager{

		static AudioSource player;

		public static void FadeSongIn(){}
		public static void FadeSongOut(){}
		public static void StopSong(){

		}

		public static void LoopAudio(){

		}

		public static void PlaySong(){

		}

		public static void CrossFadeAudio(){

		}

		static AudioSource GetPlayer(){
			GameObject obj = GameObject.FindGameObjectWithTag("MusicManager");
			return obj.GetComponent<AudioSource>();
		}

     	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void Start(){
			player=GetPlayer();
		}
	}
}