using Animation;
using Combat.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;


namespace Combat.Spawning{
	public class WaveController : MonoBehaviour{
		
		protected List<ENM_Controller> currentEnemies;

		public List<SpawnPointType[]> waves;

		protected int waveIndex = 0;

		public float staggerTime = 0f;

		public float timeBetweenWaves = 3f;

		protected DoorController[] doorsToLock;

		bool WaveCompleted(){
			if(currentEnemies.Count==0) return true;
			
			foreach(ENM_Controller enemy in currentEnemies){
				// i.e. if the enemy has been destroyed
				if(enemy==null) currentEnemies.Remove(enemy);
			}

			return false;
		}

		bool CheckForSpawningComplete() => waveIndex>=waves.Count;

		IEnumerator WaitForWaveComplete(){
			yield return new WaitUntil(() => WaveCompleted());
			waveIndex+=1;
			yield return new WaitForSeconds(timeBetweenWaves);
			if(!CheckForSpawningComplete()) StartCoroutine(HandleEnemySpawning());
		}

		IEnumerator HandleEnemySpawning(){
			SpawnPointType[] currentWave = waves[waveIndex];
			foreach(SpawnPointType spt in currentWave){
				currentEnemies.Add(Enemies.Spawning.SpawnEnemy(spt.type, spt.spawnPoint.transform.position).GetComponent<ENM_Controller>());
				yield return new WaitForSeconds(staggerTime);
			}

			StartCoroutine(WaitForWaveComplete());
		}

		// So there is no forgerring StartCoroutine()
		protected void SpawnEnemies() {
			LockDoors();
			StartCoroutine(HandleEnemySpawning());
		}

		void LockDoors(){
			if(doorsToLock.Length>0){
				foreach(DoorController dc in doorsToLock) {
					dc.locked=true;
				}
			}
		}
	}
}
