using Combat.Elements;
using Combat.Enemies;
using Entity;
using MathsAndSome;
using Player.Movement;
using UnityEngine;

namespace Entities{
	public static class DamageSphere{
		public static void CreateDamageSphere(this GameObject obj, EnemyTypes targetType, Element element, float radius=1f, float damage=10f){
			Collider[] hitObjs=Physics.OverlapSphere(obj.transform.position, radius);
			foreach(Collider col in hitObjs){
				if(targetType==EnemyTypes.player && col.isEntity<PL_Controller>()){
					mas.player.Player.TakeDamage(damage, element);
				}
				else if(targetType==EnemyTypes.enemy && col.isEntity<ENM_Controller>()){
					col.GetComponent<ENM_Controller>().TakeDamage(damage,element);
				}
			}
		}
	}
}