using UnityEngine;
using Magical;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathsAndSome;

namespace Audio{

	public class Sound{
		public GameObject player;
		public AudioSource audioSource;

		public Sound(GameObject player){
			this.player=player;
			this.audioSource=player.GetComponent<AudioSource>();
		}		
	}

	public static class AudioManager{

		public static AudioSource audioSource;
		public static GameObject audioPlayer;
		public static HandleAudioManager ham;
		public static string audioHolderName = "AudioHolder";
		public static Transform audioHolder => GameObject.Find(audioHolderName).transform; 

		public static List<Sound> players = new List<Sound>();


		public static Sound Play(AudioClip clip){
			GameObject player = GameObject.Instantiate(audioPlayer);
			player.transform.parent=audioHolder;

			Sound s = new Sound(player);
			s.audioSource.resource=clip;
			players.Add(s);
			s.audioSource.Play();
			return s;
		}

		public static async void PlaySoundUntilStop(AudioClip clip, float volume=1f, float speed=1f){
			GameObject source = GameObject.Instantiate(audioPlayer);
			Debug.Log(source.name);
			source.transform.parent=audioHolder;
			// System.Random r = new System.Random();
			source.GetComponent<AudioSource>().clip = clip;
			source.GetComponent<AudioSource>().volume = Mathf.Clamp(volume, 0, 3);
			
			source.GetComponent<AudioSource>().Play();
			await Task.Delay((int)(source.GetComponent<AudioSource>().clip.length*1000));
			GameObject.Destroy(source);
		}

		public static void Stop(Sound sound){
			sound.audioSource.Stop();
			GameObject.Destroy(sound.player);
		}

		public static void StopAll(){

		}

		// [RuntimeInitializeOnLoadMethod]
		public static void Init(){
			audioPlayer=HandleAudioManager.audioPlayer;
		}
	}

	public class HandleAudioManager : MonoBehaviour{

		public static GameObject audioPlayer=>Resources.Load<GameObject>("Resources/Prefabs/Sound/AH/SoundHolder");


		void Awake(){
			AudioManager.ham=mas.player.Player.GetComponent<HandleAudioManager>();
			AudioManager.Init();
		}

		

		void Update(){
			if(AudioManager.players.Count>0){
				for(int i = 0; i < AudioManager.players.Count ; i++){
					if(!AudioManager.players[i].audioSource.isPlaying) AudioManager.players.RemoveAt(i);
				}
			}
		}
	}
}