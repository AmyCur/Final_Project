using UnityEngine;

namespace Player{
	public class PL_SpawnController : MonoBehaviour{
		public GameObject spawnPoint;

		void Start(){
			if(spawnPoint!=null) gameObject.transform.position = spawnPoint.transform.position;
		}
	}
}