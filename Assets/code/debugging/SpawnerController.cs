using UnityEngine;
using System.Collections.Generic;
using Magical;
using Entities;

namespace Combat
{
	
	[System.Serializable]
	public struct EnemyPathPair{
		public EnemyTypes enemy;
		public string path;
	}

	public static class EnemyObjs{
		public static Dictionary<EnemyTypes, GameObject> enemyObjects = new Dictionary<EnemyTypes, GameObject>();
		public static void Print(){
			if(enemyObjects!=null){
				foreach(KeyValuePair<EnemyTypes, GameObject> kvp in enemyObjects){
					Debug.Log($"{kvp.Key} : {kvp.Value}");
				}
			}
		}
	}

	namespace Enemies{
		public static class Spawning{
			public static GameObject SpawnEnemy(EnemyTypes eType, Vector3 pos){
				EnemyObjs.Print();
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
	public class SpawnerController : PrimaryAttack{
		
		[Header("Spawner")]
		
		public EnemyTypes enemy;

		public List<EnemyPathPair> enemyPaths;
		const string basePath="Prefabs/Combat/Enemies/";

		public override bool keyDown() => magic.key.down(keys.attack);
		public override bool keyStayDown() => magic.key.gk(keys.attack);
		public override bool keyUp() => magic.key.up(keys.attack);

		void InitialiseEnemyObjects(){
			EnemyObjs.enemyObjects = new();
			foreach(EnemyPathPair e in enemyPaths){
				GameObject toSpawn = Resources.Load<GameObject>(basePath+e.path);
				Debug.Log($"Added {e.enemy}, {toSpawn?.name}");
				EnemyObjs.enemyObjects.Add(e.enemy, toSpawn);
			}
		}

		public void Start(){
			InitialiseEnemyObjects();
		}

		CombatController cc;
	
		public override void OnClick(){
			if(Physics.Raycast(pc.cameraPos, pc.fw, out RaycastHit hit, range)){
				cc ??= MathsAndSome.mas.player.Player.GetComponent<CombatController>();
				if(!(cc.spawner.assist as SpawnerMenuController).menuActivated)
					Enemies.Spawning.SpawnEnemy(enemy, hit.point);
			}
		}


	}
}