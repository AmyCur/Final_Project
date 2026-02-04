using Animation;
using Combat.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using System.Threading.Tasks;


namespace Combat.Spawning{
	public class WaveController : MonoBehaviour{
		
		protected List<GameObject> currentEnemies = new();

		public List<Wave> waves;

		protected int waveIndex = 0;

		public float staggerTime = 0f;

		public float timeBetweenWaves = 3f;

		bool waveTriggered;

		public DoorController[] doorsToLock = new DoorController[0];

		bool WaveCompleted(){
			if(currentEnemies.Count==0) return true;
			
			for(int i = 0; i < currentEnemies.Count; i++){
				if(currentEnemies[i]==null) {
					currentEnemies.Remove(currentEnemies[i]);
					break;
				}	
			}

			return false;
		}

		bool CheckForSpawningComplete() => waveIndex>=waves.Count;

		// IEnumerator WaitForWaveComplete(){
		// 	yield return new WaitUntil(() => currentEnemies.Count>0);
		// 	yield return new WaitUntil(() => WaveCompleted());
		// 	waveIndex+=1;
		// 	yield return new WaitForSeconds(timeBetweenWaves);
		// 	if(!CheckForSpawningComplete()) StartCoroutine(HandleEnemySpawning());
		// }

		async void EnableEnemies(ENM_Controller enm){
			enm.isEnabled=true;
			enm.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
			await Task.Delay(200);
		}

		bool completedSpawning=false;
		//TODO: Fix lag????? why is it lagging

		async void HandleEnemySpawning(){
			completedSpawning = false;
			Wave currentWave = waves[waveIndex];


			foreach(SpawnPointType spt in currentWave.wave){
				await Task.Delay(Mathf.FloorToInt(currentWave.delay*1000));
				GameObject ce = Enemies.Spawning.SpawnEnemy(spt.type, spt.pos);
				EnableEnemies(ce.GetComponent<ENM_Controller>());
				
				currentEnemies.Add(ce);
			}

			completedSpawning = true;
		}

		// IEnumerator HandleEnemySpawning(){
		// 	Wave currentWave = waves[waveIndex];

		// 	foreach(SpawnPointType spt in currentWave.wave){
		// 		Debug.Log("Spawning");
		// 		ENM_Controller ce = Enemies.Spawning.SpawnEnemy(spt.type, spt.spawnPoint.transform.position).GetComponent<ENM_Controller>();
		// 		Debug.Log(ce==null);
		// 		currentEnemies.Add(ce);
		// 		yield return new WaitForSeconds(currentWave.delay);
		// 	}
		// }

		// So there is no forgerring StartCoroutine()
		protected void SpawnEnemies() {
			waveTriggered=true;
			HandleEnemySpawning();
			LockDoors();
		}

		void LockDoors(){
			if(doorsToLock.Length>0){
				foreach(DoorController dc in doorsToLock) {
					dc.locked=true;
				}
			}
		}

		void Update(){
			if (waveTriggered){
				// if(completedSpawning) EnableEnemies();
				if(WaveCompleted()){
					waveIndex+=1;
					if(!CheckForSpawningComplete()) HandleEnemySpawning();
				}
			}
		}

		void Start(){
			foreach (Wave waveArray in waves){
				foreach(SpawnPointType wave in waveArray.wave){
					wave.pos = wave.spawnPoint.transform.position;
				}
			}
		}
	}
}
