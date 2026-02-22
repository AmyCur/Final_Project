using Entity;
using Player.Movement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using MathsAndSome;

namespace Environment{
	[System.Serializable]
	public class MovableObject{
		public GameObject obj;
		public Vector3 endPos;
	}

	[RequireComponent(typeof(BoxCollider))]
	public class MoveObject : MonoBehaviour{
		public List<MovableObject> objectsToMove;
		
		void DeleteTrigger() => Destroy(GetComponent<BoxCollider>());

		IEnumerator MoveObjects(){
			while(true){
				foreach(MovableObject mobj in objectsToMove){
					mobj.obj.transform.position = mas.vector.LerpVectors(mobj.obj.transform.position, mobj.endPos, Time.deltaTime);
				}

				yield return 0;
			}
		}

		void OnTriggerEnter(Collider other) {
			Debug.Log(other.name);
			if(other.isEntity<PL_Controller>()){
				DeleteTrigger();
				StartCoroutine(MoveObjects());
			}
		}
	}
}