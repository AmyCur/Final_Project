using UnityEngine;

namespace UI{
	public class QuitGame : MonoBehaviour{
		public void HandleQuitGame(){
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}
	}
}