using UnityEngine;

namespace UI{
	public class EnableOnStart:MonoBehaviour
	{
		public GameObject[] objects;


		void Start() {
			foreach (GameObject obj in objects) {
				if(obj!=null) obj.SetActive(true);
			}
		}
	}
}
