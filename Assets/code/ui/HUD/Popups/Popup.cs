using System.Collections;
using UnityEngine;

namespace FileManagement.UI{
	public class Popup : MonoBehaviour{
		RectTransform trans;
		[SerializeField] float initScale=0f;
		[SerializeField] float targetScale=1f;
		public Coroutine sizeRoutine;

		public IEnumerator ChangeSize(bool grow=true){
			while(grow ? (trans.localScale.y < targetScale-0.01f) : trans.localScale.y > initScale+0.01f){
				trans.localScale=new Vector2(trans.localScale.x, Mathf.Lerp(trans.localScale.y, grow ? targetScale : initScale, Time.deltaTime*10f));
				yield return 0;
			}	
			trans.localScale=new Vector2(trans.localScale.x, grow ? targetScale : initScale);
		}

		public virtual void Start() {
			trans=GetComponent<RectTransform>();	
			trans.localScale=new Vector2(trans.localScale.x, initScale);
		}
	}
}