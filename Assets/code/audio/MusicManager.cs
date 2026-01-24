using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

namespace FileManagement.Audio{

	[System.Serializable]
	public class Audio{
		public AudioClip clip;
	}

	public class AudioObject
	{
		public GameObject obj;
		public AudioSource src;

		public AudioObject(GameObject obj)
		{
			this.obj = obj;
			src=this.obj.GetComponent<AudioSource>();
		}
	}
	
	// Im too depressed for ts
	/*
		!Problems
		* Audio has to be of 1 volume
		* You can spam audios
		* The songs ive made are arse
	*/

	public static class MusicManager{

		static GameObject player;
		static AudioObject currentAudio;
		public static LevelAudio currentLevelAudio;

		public static void FadeSongIn(){}
		public static void FadeSongOut(){}
		public static void StopSong(){
			currentAudio.src.Stop();
			GameObject.Destroy(currentAudio.obj);
		}

		public static void LoopAudio(){

		}

		public static void PlaySong(AudioClip targetClip){
			AudioObject pl = new (GameObject.Instantiate(player));
			pl.src.clip = targetClip;
			pl.src.Play();
			currentAudio=pl;
		}

		public static IEnumerator CrossFadeAudio(AudioClip targetClip){
			AudioObject pl = new (GameObject.Instantiate(player));
			pl.src.clip = targetClip;
			pl.src.volume=0f;
			pl.src.Play();
			pl.src.time=currentAudio.src.time;

			float baseVolume = currentAudio.src.volume;
			
			for(int i = 0; i < 1000; i++)
			{
				//FIXME: Only works if volume = 1 rn!
				currentAudio.src.volume=baseVolume-i/1000f;
				pl.src.volume=i/1000f;
				yield return new WaitForSeconds(0.001f);
			}

			StopSong();
			GameObject.Destroy(currentAudio.obj);
			currentAudio=pl;					
		}

		static GameObject GetPlayer(){
			return Resources.Load<GameObject>("Prefabs/Sound/MusicPlayer");
		}

		static LevelAudio GetCurrentLevelAudio()
		{
			return Resources.Load<LevelAudio>($"Scriptables/Audio/{SceneManager.GetActiveScene().name}");
		}


     	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void Start(){
			player=GetPlayer();
			currentLevelAudio=GetCurrentLevelAudio();
			PlaySong(currentLevelAudio.baseClip);
		}
	}
}