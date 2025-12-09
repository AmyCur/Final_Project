using UnityEngine;
using EntityLib;

public class UnloadOjbs : MonoBehaviour{

	public GameObject[] objsToUnload;

	void UnloadObjects(){
		if(objsToUnload.Length == 0) return;

		foreach(GameObject obj in objsToUnload){
			obj.SetActive(false);
		}
	}


	void OnTriggerEnter(Collider other){
		if(other.isEntity(typeof(Player.PL_Controller))) UnloadObjects();
	}
}