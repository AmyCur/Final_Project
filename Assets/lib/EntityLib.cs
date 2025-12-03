using Elements;
using System;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using System.Linq;
using MathsAndSome;
using Globals;

namespace EntityLib {
	public static class Entity {
		static Dictionary<Type, string> enemyTypeToName = new() {
			{typeof(Player.PL_Controller), "player"},
			{typeof(ENM_Controller), "enemy"},
			{typeof(ENT_Controller), "entity"},
		};

		static void KillPlayer(){ 
			Player.PL_Controller pc = mas.player.GetPlayer();
			pc.health.h-=pc.health.h;
		}

		static void KillEnemies(){
			GameObject[] enemies = GameObject.FindGameObjectsWithTag(glob.enemyTag);
			foreach(GameObject obj in enemies){
				ENM_Controller e = obj.GetComponent<ENM_Controller>();
				e.health.h-=e.health.h;
			}
		}

		public static void KillAll(Type targetType=null){
			if(targetType==null){
				KillEnemies();
				KillPlayer();
			}
			else if(targetType == typeof(Player.PL_Controller)) KillPlayer();
			else if(targetType == typeof(ENM_Controller)) KillEnemies();


		}
		// This is so ugly and i hate it but i actually cant figure out another way because i am stupid
		public static bool isEntity(this object m, Type targetType = null) {
			targetType ??= typeof(ENT_Controller);

			if (targetType == typeof(Player.PL_Controller)) {
				if (m is RaycastHit h) {
					return h.collider.CompareTag(Globals.glob.playerChildTag) || !!h.collider.GetComponent<Player.PL_Controller>() || h.collider.CompareTag(Globals.glob.playerTag);
				}
				else if (m is Collider c) return (c.GetComponent<Player.PL_Controller>()) != null || c.CompareTag(Globals.glob.playerChildTag);
				else if (m is MonoBehaviour mono) return mono is Player.PL_Controller;
				return false;
			}

			else if (targetType == typeof(ENM_Controller)) {
				if (m is RaycastHit h) return h.collider.tag == Globals.glob.enemyTag || !!h.collider.GetComponent<ENM_Controller>();
				else if (m is Collider c) return !!c.GetComponent<ENM_Controller>();
				else if (m is MonoBehaviour mono) return mono is ENM_Controller;
				return false;
			}

			else if (targetType == typeof(ENT_Controller)) {

				if (m is RaycastHit h) return !!(h.collider.GetComponent<ENT_Controller>()) || h.collider.CompareTag(Globals.glob.playerChildTag);
				else if (m is Collider c) return !!(c.GetComponent<ENT_Controller>()) || c.CompareTag(Globals.glob.playerChildTag);
				else if (m is MonoBehaviour mono) return mono is ENT_Controller;
				return false;
			}

			return false;
		}

		public static bool inRange(this float v, float minRange, float maxRange) {
			return v >= minRange && v < maxRange;
		}

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
}