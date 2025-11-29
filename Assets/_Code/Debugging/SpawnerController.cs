using UnityEngine;
using System.Collections.Generic;

namespace Combat
{
	public enum Enemy{
		melee,
		ranged
	}


	[System.Serializable]
	public struct EnemyPathPair{
		public Enemy enemy;
		public string path;
	}

	public static class EnemyObjs{
		public static Dictionary<Enemy, GameObject> enemyObjects;
	}



	namespace Enemies{
		public static class Spawning{
			public static GameObject SpawnEnemy(Enemy eType, Vector3 pos){
				GameObject obj=EnemyObjs.enemyObjects[eType] ;

				if(obj!=null){
					GameObject enemy=GameObject.Instantiate(obj, pos, Quaternion.identity);
					enemy.transform.parent = GameObject.FindWithTag("Enemies").transform;
					return enemy;
				}
				else Debug.Log($"enemyObjects does not contain {eType}");
				return null;
			}
		}
	}

	[CreateAssetMenu(fileName = "Spawner Controller", menuName = "Attacks/Create/Spawner")]
	public class SpawnerController : SingularAttack{
		
		[Header("Spawner")]
		
		[SerializeField] Enemy enemy;

		public List<EnemyPathPair> enemyPaths;
		const string basePath="Combat/Enemies/";

		void OnValidate(){
			
		}

		void InitialiseEnemyObjects(){
			foreach(EnemyPathPair e in enemyPaths){
				EnemyObjs.enemyObjects.Add(e.enemy, Resources.Load<GameObject>(basePath+e.path));
			}
		}

		void Start(){
			InitialiseEnemyObjects();
		}
	
		public override void OnClick(){
			if(Physics.Raycast(pc.cameraPos, pc.fw, out RaycastHit hit, range)){
				Enemies.Spawning.SpawnEnemy(enemy, hit.point);
			}
		}


	}
}