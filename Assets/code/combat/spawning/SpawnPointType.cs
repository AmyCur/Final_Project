using Entities;
using UnityEngine;

namespace Combat.Spawning{
	[System.Serializable]
	public class SpawnPointType{
		public EnemySpawnPoint spawnPoint;
		public Vector3 pos;
		public EnemyTypes type;
	}
}

