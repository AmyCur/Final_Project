using MathsAndSome;
using UnityEditor;
using UnityEngine;

namespace TD{
	public class LookAtPlayer : MonoBehaviour
	{
		Transform player;
		[SerializeField] Vector3 offset;


		void LookAtTarget(Transform target){
			transform.LookAt(target);
			transform.localEulerAngles=transform.localEulerAngles+offset;
		}

		void Update() => LookAtTarget(player);

		void Start() {
			player = mas.player.Player.transform;
		}

	}
}

