using UnityEngine;

namespace Combat.Spawning{
	public class EnemySpawnPoint : MonoBehaviour{
		public void RemoveObject() => Destroy(gameObject);
	}
}