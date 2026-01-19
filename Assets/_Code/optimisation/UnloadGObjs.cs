using UnityEngine;
using Entities;

public class UnloadOjbs : MonoBehaviour{

	public GameObject[] objsToUnload;

	void UnloadObjects(){
		if(objsToUnload.Length == 0) return;

		foreach(GameObject ob in objsToUnload){
			ob.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.CompareTag("PlayerChild")) UnloadObjects();
	}
}