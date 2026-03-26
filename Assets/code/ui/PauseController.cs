using UnityEngine;
using Magical;
using System;
using UnityEngine.SceneManagement;

namespace UI{
	public static class Pause{
		public static bool paused;
	}
	public class PauseController : MonoBehaviour
	{

		[SerializeField] GameObject pauseMenu;

		void TogglePaused()
		{
			Pause.paused = !Pause.paused;
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
