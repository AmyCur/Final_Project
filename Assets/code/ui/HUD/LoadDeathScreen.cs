using MathsAndSome;
using System.Collections;
using UnityEngine;

namespace UI.HUD{
	public class LoadDeathScreen : MonoBehaviour
	{
		RectTransform trans;
		Player.Movement.PL_Controller pc;
		[SerializeField] float targetScale = 1f;
		[SerializeField] float lerpSpeed = 5f;

		IEnumerator Grow(){
			while(trans.localScale.y != targetScale){
				trans.localScale=new Vector2(trans.localScale.x, Mathf.Lerp(trans.localScale.y, targetScale, Time.deltaTime*lerpSpeed));
				yield return 0;
			}		
		}

		void Start(){
			pc=mas.player.Player;
			if(TryGetComponent<RectTransform>(out trans)){
				trans.localScale=new(trans.localScale.x, 0);
			}
		}

		void Update() {
			if(pc.state==Player.PlayerState.dead){
				StartCoroutine(Grow());
			}
		}
	}
}
