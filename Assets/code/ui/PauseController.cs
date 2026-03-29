using UnityEngine;
using Magical;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace UI{
	public static class Pause{
		public static bool paused;
	}

	public class PauseController : MonoBehaviour
	{

		[SerializeField] GameObject pauseMenu;
		[SerializeField] float targetFrequency=400f;
		[SerializeField] float normalFrequency=22000f;

		Coroutine lowPassRoutine;

		IEnumerator LowPassMusic(){
			List<AudioLowPassFilter> alps= new();

			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("MusicPlayer")){
				alps.Add(obj.GetComponent<AudioLowPassFilter>());
			}


			// List<bool> allCorrect=new();
			// bool ac=true;

			// foreach(AudioLowPassFilter alp in alps){
			// 	if(!(Pause.paused ? alp.cutoffFrequency > targetFrequency+10 : alp.cutoffFrequency < normalFrequency-10)){
			// 		ac=false;
			// 		break;
			// 	}
			// }

			foreach(AudioLowPassFilter alp in alps){
				for(int i = 0; i < 10000; i++){
					alp.cutoffFrequency=Mathf.Lerp(alp.cutoffFrequency, Pause.paused ? targetFrequency : normalFrequency, Time.deltaTime*10);
					yield return 0;
				}
					
				
				yield return 0;

			}
			yield return 0;




			// AudioLowPassFilter alp = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioLowPassFilter>();
			// bool condition = Pause.paused ? Mathf.RoundToInt(alp.cutoffFrequency)<=targetFrequency+10 : Mathf.RoundToInt(alp.cutoffFrequency)>=normalFrequency-10;
			// while(!condition){
			// 	alp.cutoffFrequency=Mathf.Lerp(alp.cutoffFrequency, Pause.paused ? targetFrequency : normalFrequency, Time.deltaTime*5f);
			// 	yield return 0;
			// }
		}

		void TogglePaused()
		{
			
			Pause.paused = !Pause.paused;

			if(lowPassRoutine!=null) StopCoroutine(lowPassRoutine);
			lowPassRoutine=StartCoroutine(LowPassMusic());

			Time.timeScale = Pause.paused ? 0 : 1;
			Cursor.lockState= Pause.paused ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = Pause.paused;
			PlayerPrefs.SetInt(nameof(Pause.paused), Convert.ToInt32(Pause.paused));
			pauseMenu.SetActive(Pause.paused);
		}

		// Update is called once per frame
		void Update()
		{
			if (magic.key.down(keys.pause))
			{
				TogglePaused();	
			}
		}

		public void Restart(){
			HandleRestart.Restart();
			TogglePaused();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public void Resume()=> TogglePaused();

		public void QuitGame()
		{
			Application.Quit();
		}

		public void QuitToMainMenu()
		{
			HandleRestart.Restart();
			TogglePaused();
			SceneManager.LoadScene(0);
		}
	}
}
