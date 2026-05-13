using UnityEngine;
using Magical;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

namespace UI {
	public static class Pause {
		public static bool paused;
	}

	public class PauseController : MonoBehaviour {

		[SerializeField] GameObject pauseMenu;
		[SerializeField] float targetFrequency = 400f;
		[SerializeField] float normalFrequency = 22000f;

		Coroutine lowPassRoutine;

		IEnumerator LowPassMusic() {
			AudioLowPassFilter alpf = GameObject.Find("LevelMusicPlayer").GetComponent<AudioLowPassFilter>();
			GameObject.Find("LevelMusicPlayer").GetComponent<AudioHighPassFilter>().cutoffFrequency = Pause.paused ? 500 : 10;
			float target = Pause.paused ? targetFrequency : normalFrequency;
			alpf.cutoffFrequency = target;
			// while (alpf.cutoffFrequency != targetFrequency) {
			// 	alpf.cutoffFrequency = Mathf.Lerp(alpf.cutoffFrequency, target, Time.deltaTime);
			// 	Debug.Log("lerping");
			// 	yield return 0;
			// }
			// Debug.Log("done lerp");
			//
			yield return 0;
		}

		void TogglePaused() {

			Pause.paused = !Pause.paused;

			if (lowPassRoutine != null) StopCoroutine(lowPassRoutine);
			lowPassRoutine = StartCoroutine(LowPassMusic());

			Time.timeScale = Pause.paused ? 0 : 1;
			Cursor.lockState = Pause.paused ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = Pause.paused;
			PlayerPrefs.SetInt(nameof(Pause.paused), Convert.ToInt32(Pause.paused));
			pauseMenu.SetActive(Pause.paused);
		}

		// Update is called once per frame
		void Update() {
			if (magic.key.down(keys.pause) && MathsAndSome.mas.player.Player.cutsceneOver) {
				TogglePaused();
			}
		}

		public void Restart() {
			HandleRestart.Restart();
			TogglePaused();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public void Resume() => TogglePaused();

		public void QuitGame() {
			Application.Quit();
		}

		public void QuitToMainMenu() {
			HandleRestart.Restart();
			TogglePaused();
			SceneManager.LoadScene(0);
		}
	}
}