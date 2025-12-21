using UnityEngine;
using MathsAndSome;
using System.Collections;

namespace Player{
	public class CameraController : MonoBehaviour
	{
		Camera camera => Camera.main;
		public float baseFOV=90f;
		public float maxFOV=120f;
		[SerializeField] float lerpSpeed=10f;
		float speed => mas.player.Player.rb.linearVelocity.magnitude;

		IEnumerator SetFOV(){
			while(true){
				float targetFOV=Mathf.Clamp(baseFOV+(Mathf.Clamp(speed/5f, 0f, 50f)), baseFOV, maxFOV);
				camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, Time.deltaTime * lerpSpeed * (Mathf.FloorToInt(speed) > 8 ? 1f : 5f));
				yield return 0;
			}	
		}

		void SetFOVBasedOnSpeed(){
		
		}

		void Update(){
			Debug.Log(speed);
		}

		IEnumerator WaitForReady(){
			yield return new WaitUntil(() => mas.player.Player.rb !=null);
			StartCoroutine(SetFOV());
		}

		void Start(){
			StartCoroutine(WaitForReady());
		}
	}
}
