using Combat.Elements;
using System;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using System.Linq;
using MathsAndSome;
using Globals;
using Combat.Enemies;
using Combat.Attacks.Projectiles;

namespace Entity {
	public static class Entity {
		static Dictionary<Type, string> enemyTypeToName = new() {
			{typeof(Player.Movement.PL_Controller), "player"},
			{typeof(ENM_Controller), "enemy"},
			{typeof(ENT_Controller), "entity"},
		};

		public static void KillAll(Type targetType=null){
			if(targetType==null){
				Enemy.KillEnemies();
				PlayerUtils.KillPlayer();
			}
			else if(targetType == typeof(Player.Movement.PL_Controller)) PlayerUtils.KillPlayer();
			else if(targetType == typeof(ENM_Controller)) Enemy.KillEnemies();
		}

		public static bool isGrounded(this object obj, float range){
			if(obj is Collider col){
				return Physics.Raycast(col.transform.position, Vector3.down, range);
			}
			else if(obj is GameObject go){
				return Physics.Raycast(go.transform.position, Vector3.down, range);
			}
			else if(obj is MonoBehaviour mo){
				return Physics.Raycast(mo.gameObject.transform.position, Vector3.down, range);
			}
			return false;
		}


		// This is so ugly and i hate it but i actually cant figure out another way because i am stupid
		// Edit: I figured out another way which is a bit nicer :) i love generics but i hate myself

		
		public static bool isEntity(this object m){
			if (m is RaycastHit h) return !!(h.collider.GetComponent<ENT_Controller>()) || h.collider.CompareTag(Globals.glob.playerChildTag);
			else if (m is Collider c) return !!(c.GetComponent<ENT_Controller>()) || c.CompareTag(Globals.glob.playerChildTag);
			else if (m is MonoBehaviour mono) return mono is ENT_Controller;
			return false;
		}
		
		public static bool isEntity<T>(this object m){
			Type targetType = typeof(T);
			targetType ??= typeof(ENT_Controller);

			if (targetType == typeof(Player.Movement.PL_Controller)) {
				if (m is RaycastHit h) {
					
					return h.collider.CompareTag(Globals.glob.playerChildTag) || !!h.collider.GetComponent<Player.Movement.PL_Controller>() || h.collider.CompareTag(Globals.glob.playerTag);
				}

				else if (m is Collider c){
					bool isPlayer = c.GetComponent<Player.Movement.PL_Controller>() != null;
					bool isPlayerTag = c.CompareTag(Globals.glob.playerChildTag);
					bool isPlayerChild = c.transform.parent != null ? c.transform.parent.GetComponent<Player.Movement.PL_Controller>()!=null : false;
					return (isPlayer || isPlayerTag || isPlayerChild) && c.GetType()!=typeof(BoxCollider);
				}
				else if (m is MonoBehaviour mono) return mono is Player.Movement.PL_Controller;
				return false;
			}

			else if (targetType == typeof(ENM_Controller)) {
				if (m is RaycastHit h) return h.collider.tag == Globals.glob.enemyTag || !!h.collider.GetComponent<ENM_Controller>();
				else if (m is Collider c) return !!c.GetComponent<ENM_Controller>();
				else if (m is MonoBehaviour mono) return mono is ENM_Controller;
				return false;
			}

			else if (targetType == typeof(ENT_Controller)) return m.isEntity();
			
			return false;
		}

		public static bool isProjectile(this object p){
			if(p is RaycastHit h){
				if(h.collider.CompareTag(glob.projectileTag)) return true;
			}

			if(p is Collider c){
				if(c.CompareTag(glob.projectileTag)) return true;
			}

			return false;
		}
		
		// Horizontal
		public static bool inRange3D(this GameObject obj, GameObject targetObject, float range){			
			return (targetObject.transform.localPosition - obj.transform.localPosition).magnitude <= range;
		}

		public static bool inRange2D(this GameObject obj, GameObject targetObject, float range)
		{
			float distance = new Vector2(
				targetObject.transform.localPosition.x-obj.transform.localPosition.x,
				targetObject.transform.localPosition.z-obj.transform.localPosition.z
			).magnitude;

			return distance <= range;
		}

	
		
		// public static bool inRange(this float v, float minRange, float maxRange) {
		// 	return v >= minRange && v < maxRange;
		// }

		public static List<GameObject> GetAllEnemies() => GameObject.FindGameObjectsWithTag(Globals.glob.enemyTag).ToList();

		public static GameObject GetNearestEnemy(this GameObject obj, float range, out ENM_Controller enemy){
			Vector3 position = obj.transform.position;
			List<GameObject> enemies = GetAllEnemies();
			
			GameObject nearest=null;
			float distance=1_000_000f;

			foreach(GameObject enem in enemies){
				if(Physics.Raycast(position, (enem.transform.position-position).normalized, out RaycastHit hit, range))	{
					if(hit.distance < distance && hit.isEntity()){
						distance=hit.distance;
						nearest=enem;
					}
				}
			}

			if(nearest!=null) 
				enemy=nearest.GetComponent<ENM_Controller>();
			else enemy=null;

			return nearest;
			
		}
	}

	public static class PlayerUtils{
		public static void ShootPlayer<T>(this GameObject obj, float range, float damage){
			if(typeof(T) == typeof(RaycastHit)){
				if(Physics.Raycast(obj.transform.position, mas.player.Player.transform.position-obj.transform.position, out RaycastHit hit, range)){
					if (hit.isEntity<Player.Movement.PL_Controller>()){
						mas.player.Player.health-=damage;
					}
				}
			}

			if(typeof(T) == typeof(BulletController)){
				GameObject projectile = Resources.Load<GameObject>("Prefabs/Combat/Projectiles/Bullet");
				GameObject proj = GameObject.Instantiate(projectile, obj.transform.position, Quaternion.identity);
				proj.GetComponent<BulletController>().Init((mas.player.Player.transform.position-obj.transform.position).normalized, damage);
				proj.transform.parent=GameObject.FindWithTag("Bullets").transform;
			}
			
		}

		

		public static void KillPlayer() => mas.player.Player.health -= mas.player.Player.health;
	}

	public static class Enemy{
		public static void KillEnemies(){
			GameObject[] enemies = GameObject.FindGameObjectsWithTag(glob.enemyTag);
			foreach(GameObject obj in enemies){
				ENM_Controller e = obj.GetComponent<ENM_Controller>();
				e.health-=e.health;
			}
		}
	}
}