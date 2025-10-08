using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UIManager
{
    [System.Serializable]
    public class Popup
    {
        public Image screen;
        public bool inflated;
        public Coroutine inflationRoutine;
        public Vector2 targetSize;
        [SerializeField] Vector2 t;
		[SerializeField] float speed = 5f;

        public IEnumerator LerpSize(Vector2 targetSize) {
			Debug.Log("Lerping");

			while (screen.transform.localScale.x != targetSize.x || screen.transform.localScale.y != targetSize.y) {
				this.t.x = Mathf.Lerp(this.t.x, targetSize.x, Time.deltaTime * this.speed);
				this.t.y = Mathf.Lerp(this.t.y, targetSize.y, Time.deltaTime * this.speed);
				this.screen.transform.localScale = t;
				yield return 0;
			}
		}


    }

    public static class Create
    {
		public static void CreatePopup(MonoBehaviour m, Popup popup) {
			Debug.Log("create");
			if (popup.inflationRoutine != null) m.StopCoroutine(popup.inflationRoutine);
			popup.inflationRoutine = m.StartCoroutine(popup.LerpSize(popup.inflated ? Vector2.zero : popup.targetSize));
			popup.inflated = !popup.inflated;
            
        }
    }
}