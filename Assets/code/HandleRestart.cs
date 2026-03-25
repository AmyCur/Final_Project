using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class HandleRestart{
	public static void Restart(){
		if(PlayerPrefs.HasKey(PL_CPController.CPPKey)) PlayerPrefs.DeleteKey(PL_CPController.CPPKey);
	}


	[RuntimeInitializeOnLoadMethod]
	static void RestartIfTutorial(){
		if(SceneManager.GetActiveScene().buildIndex==0){
			Restart();
		}
	}
}