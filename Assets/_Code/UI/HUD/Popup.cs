using System.Collections;
using UnityEngine;

namespace Cur.UI{
	public abstract class Popup : MonoBehaviour{

		RectTransform trans;
		[SerializeField] float initScale=0f;
		[SerializeField] float targetScale=1f;

		public IEnumerator ChangeSize(bool grow=true){
			while(trans.localScale.y != (grow ? targetScale : initScale)){
				trans.localScale=new Vector2(trans.localScale.x, Mathf.Lerp(trans.localScale.y, grow ? targetScale : initScale, Time.deltaTime*10f));
				yield return 0;
			}		
		}

		public virtual void Start() {
			trans=GetComponent<RectTransform>();	
			trans.localScale=new Vector2(trans.localScale.x, initScale);
		}


	}
}