using UnityEngine;
using MathsAndSome;
using System.Collections;

namespace Player{
	public class CameraController : MonoBehaviour
	{

		[SerializeField] float baseFOV=90f;
		[Header("Breathing")]
		[SerializeField] Vector2 breathingRange=new Vector2(-10,10);
		[SerializeField] float breathingSpeed = 3f;
		[SerializeField] bool breathingIn;
		Camera camera=>mas.player.Player.playerCamera;


		IEnumerator Breathe(){
			while(true){
				float targetFOV=baseFOV+(breathingIn ? breathingRange[0] : breathingRange[1]);
				
				while(breathingIn ? camera.fieldOfView-0.1f>=targetFOV : camera.fieldOfView+0.1f<=targetFOV){
						camera.fieldOfView=Mathf.Lerp(camera.fieldOfView, targetFOV, Time.deltaTime*breathingSpeed);
						yield return 0;
				}
				
				breathingIn=!breathingIn;
				yield return 0;
			}
		}

		void Start() {
			StartCoroutine(Breathe());
		}




	}
}
