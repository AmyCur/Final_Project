using Entity;
using MathsAndSome;
using Player.Movement;
using System.Collections;
using UnityEngine;

namespace Animation{
	public class DoorController : MonoBehaviour
	{
		Vector3 startPos;
		Coroutine routine;
		[SerializeField] float doorOpenSpeed = 10f;
		[HideInInspector] public bool locked;
		bool canMove => !locked;

		IEnumerator MoveDoorRoutine(Vector3 targetPosition){
			Vector3 startPosition=transform.position;

			while(transform.position!=targetPosition){
				transform.position = mas.vector.LerpVectors(transform.position, targetPosition, Time.deltaTime*doorOpenSpeed);
				yield return 0;
			}
		}

		void MoveDoor(Vector3 targetPosition){
			if(routine!=null) StopCoroutine(routine);
			routine = StartCoroutine(MoveDoorRoutine(targetPosition));
		}

		void OnTriggerEnter(Collider other){
			if(other.isEntity<PL_Controller>() && canMove){
				MoveDoor(startPos+(Vector3.up*5f));
			}
		}

		void OnTriggerExit(Collider other){
			if(other.isEntity<PL_Controller>() && canMove){
				MoveDoor(startPos);
			}
		}

		void Start(){
			startPos=transform.position;
		}
	}
}
