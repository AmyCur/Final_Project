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

        public IEnumerator LerpSize(Vector2 targetSize, float speed = 5f)
        {
            while (Mathf.Abs(screen.transform.localScale.x - targetSize.x) < 0.1f || Mathf.Abs(screen.transform.localScale.y - targetSize.y) < 0.1f)
            {
                this.t.x = Mathf.Lerp(this.t.x, targetSize.x, Time.deltaTime * speed);
                this.t.y = Mathf.Lerp(this.t.y, targetSize.y, Time.deltaTime * speed);
                this.screen.transform.localScale = t;
                yield return 0;
            }
        }


    }

    public static class Create
    {
        public static void CreatePopup(MonoBehaviour m,Popup popup)
        {
            if (popup.inflationRoutine != null) m.StopCoroutine(popup.inflationRoutine);
            popup.inflationRoutine = m.StartCoroutine(popup.LerpSize(popup.inflated ? Vector2.zero : popup.targetSize));
            
        }
    }
}