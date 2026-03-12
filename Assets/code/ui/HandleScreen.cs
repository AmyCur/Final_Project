using MathsAndSome;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI{
	public class HandleScreen : MonoBehaviour{

		public GameObject screen;
		public bool open;
		[SerializeField] float animationSpeed=10f;
		Animator ac => GameObject.Find("ButtonsCanvas").GetComponent<Animator>();
		
		void Start(){
			// screen.transform.localScale=new(
			// 	screen.transform.localScale.x,
			// 	open ? 1 : 0,
			// 	screen.transform.localScale.z
			// );

			ac.SetBool("zoom_in", false);
			ac.SetBool("zoom_out", false);
		}

		public IEnumerator CloseScreen(){
			HandleAllMenus.openScreens.Remove(this);
			open=!open;
			ac.SetBool("zoom_in", true);
			ac.SetBool("zoom_out", false);
			
			while(screen.transform.localScale.y > 0){
				screen.transform.localScale=mas.vector.LerpVectors(
					screen.transform.localScale,
					new Vector3(
						screen.transform.localScale.x,
						-0.1f,
						screen.transform.localScale.z
					),

					Time.deltaTime*animationSpeed
				);

				yield return 0;
			}

			// screen.transform.localScale=new(screen.transform.localScale.x, 0, screen.transform.localScale.z);
		}

		public IEnumerator OpenScreen(){
			HandleAllMenus.CloseOpenScreens();
			HandleAllMenus.openScreens.Add(this);
			open=!open;
			ac.SetBool("zoom_in", false);
			ac.SetBool("zoom_out", true);
			// GameObject.Find("MainScreen").SetActive(false);
			// screen.SetActive(true);


			while(screen.transform.localScale.y < 1){
				screen.transform.localScale=mas.vector.LerpVectors(
					screen.transform.localScale,
					new Vector3(
						screen.transform.localScale.x,
						1.1f,
						screen.transform.localScale.z
					),

					Time.deltaTime*animationSpeed
				);

				yield return 0;
			}


			// screen.transform.localScale=new(screen.transform.localScale.x, 1, screen.transform.localScale.z);
		}

		Coroutine screenAnimation;

		public void ChangeScreenState(){
			if(screenAnimation!=null) StopCoroutine(screenAnimation);
			if (open) screenAnimation = StartCoroutine(CloseScreen());
			else screenAnimation = StartCoroutine(OpenScreen());
		}
	}
}