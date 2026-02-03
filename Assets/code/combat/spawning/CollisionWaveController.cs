using Entity;
using Player.Movement;
using UnityEngine;

namespace Combat.Spawning{
	[RequireComponent(typeof(BoxCollider))]
	public class CollisionWaveController : WaveController{
		void OnTriggerEnter(Collider other) {
			if(other.isEntity<PL_Controller>()){
				GetComponent<BoxCollider>().enabled=false;
				SpawnEnemies();
			}
		}
	}
}