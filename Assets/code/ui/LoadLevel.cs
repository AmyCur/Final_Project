using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] int levelIndex;
    public void HandleLoadLevel()  {
		Debug.LogWarning($"Loading {levelIndex}");
		SceneManager.LoadScene(levelIndex);
	}
}
