using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FileManagement.UI {
	public static class Props{
		public static bool inMenu;
	}


	public static class ColorUtil {
		// intesity should be between 0 and 1
		public static Color Darken(this Color color, float intesity = 0.2f) {
			return new Color(
				Mathf.Lerp(color.r, 0, intesity),
				Mathf.Lerp(color.g, 0, intesity),
				Mathf.Lerp(color.b, 0, intesity)
			);
		}

		public static Color Lighten(this Color color, float intesity = 0.2f) {
			return new Color(
				Mathf.Lerp(color.r, 1, intesity),
				Mathf.Lerp(color.g, 1, intesity),
				Mathf.Lerp(color.b, 1, intesity)
			);
		}

		public static Color Lerp(Color color, Color targetColor, float intesity) {
			return new Color(
				Mathf.Lerp(color.r, targetColor.r, intesity),
				Mathf.Lerp(color.g, targetColor.g, intesity),
				Mathf.Lerp(color.b, targetColor.b, intesity)
			);
		}
	}

	// [System.Serializable]
    // public class Popup
    // {
    //     public Image screen;
    //     public bool inflated;
    //     public Coroutine inflationRoutine;
    //     public Vector2 targetSize;
    //     [SerializeField] Vector2 t;
	// 	[SerializeField] float speed = 5f;

    //     public IEnumerator LerpSize(Vector2 targetSize) {
	// 		Debug.Log("Lerping");

	// 		while (screen.transform.localScale.x != targetSize.x || screen.transform.localScale.y != targetSize.y) {
	// 			this.t.x = Mathf.Lerp(this.t.x, targetSize.x, Time.deltaTime * this.speed);
	// 			this.t.y = Mathf.Lerp(this.t.y, targetSize.y, Time.deltaTime * this.speed);
	// 			this.screen.transform.localScale = t;
	// 			yield return 0;
	// 		}
	// 	}


    // }

    // public static class Create
    // {
	// 	public static void CreatePopup(MonoBehaviour m, Popup popup) {
	// 		Debug.Log("create");
	// 		if (popup.inflationRoutine != null) m.StopCoroutine(popup.inflationRoutine);
	// 		popup.inflationRoutine = m.StartCoroutine(popup.LerpSize(popup.inflated ? Vector2.zero : popup.targetSize));
	// 		popup.inflated = !popup.inflated;
            
    //     }
    // }
}