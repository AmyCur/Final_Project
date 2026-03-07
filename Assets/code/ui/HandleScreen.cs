using UnityEngine;

namespace UI{
	public class HandleScreen : MonoBehaviour{
		public GameObject screen;
		public bool open;
		
		void Start(){
			screen.transform.localScale=new(
				screen.transform.localScale.x,
				open ? 1 : 0,
				screen.transform.localScale.z
			);
		}

		public void CloseScreen(){

		}

		public void OpenScreen(){

		}

		public void ChangeScreenState(){
			if(open) CloseScreen();
			else OpenScreen();
		}
	}
}