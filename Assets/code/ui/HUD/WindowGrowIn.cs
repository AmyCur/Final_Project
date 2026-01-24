using MathsAndSome;
using System.Collections;
using UnityEngine;

namespace FileManagement.UI{
	public class WindowGrowIn : MonoBehaviour
	{
		RectTransform trans;
		Player.Movement.PL_Controller pc;
		[SerializeField] float delay=1f;
		[SerializeField] float targetScale = 1f;
		[SerializeField] bool closeOnDeath=true;
		float initScale=0f;

		IEnumerator Grow(){
			yield return new WaitForSeconds(delay);
			while(trans.localScale.y != targetScale){
				trans.localScale=new Vector2(trans.localScale.x, Mathf.Lerp(trans.localScale.y, targetScale, Time.deltaTime*10f));
				yield return 0;
			}		
		}

		void Start(){
			pc=mas.player.Player;

			if(TryGetComponent<RectTransform>(out trans)){
				trans.localScale=new Vector2(trans.localScale.x, initScale);
				StartCoroutine(Grow());
			}
		}

		void Update() {
			if(pc.state==Player.PlayerState.dead && closeOnDeath){
				delay/=3f;
				targetScale=0f;
				StartCoroutine(Grow());
			}
		}


	}
}
