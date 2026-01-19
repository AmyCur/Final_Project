using UnityEngine;
using System.Collections.Generic;
using Magical;
using Combat.Enemies;

namespace Combat.Attacks{
	[CreateAssetMenu(fileName = "New CRT Attack", menuName = "Attacks/Create/CRT Attack")]
	public class CRTAttack : AlternateAttack {
		[Header("Stats")]

		public float attackRadius = 3f;
		[Range(1, 1000)] public int maxEnemies = 10;
		public float staggerTime = 1f;
		public GameObject crt;

		List<GameObject> FindAllEnemies() => Entities.Entity.GetAllEnemies();

		List<GameObject> FindAllEnemiesInRadius(){
			List<GameObject> targetedObjs = new();
			if(FindAllEnemies().Count > 0)
			{
				foreach(GameObject obj in FindAllEnemies()){
					if(Physics.Raycast(pc.transform.position, (obj.transform.position-pc.transform.position).normalized, out RaycastHit hit, attackRadius)){
						targetedObjs.Add(hit.collider.gameObject);
					}
				}
			}
			
			return targetedObjs;
		}

		void CreateCRTAbove(GameObject obj){
			GameObject crtObj = Instantiate(crt, obj.transform.position+(Vector3.up*10f), Quaternion.identity);
			crtObj.GetComponent<CRTController>().damage=damage;
			pc.StartCoroutine(obj.GetComponent<ENM_Controller>().Stagger(staggerTime));
		}

		public override bool keyDown() => magic.key.down(keys.ability);
		public override bool keyStayDown() => magic.key.gk(keys.ability);
		public override bool keyUp() => magic.key.up(keys.ability);

        public override void OnClick(){
			List<GameObject> enemies = FindAllEnemiesInRadius();
			if(enemies.Count > 0){
				foreach(GameObject obj in enemies){
					CreateCRTAbove(obj); 
				}
			}

            if(enemies.Count > 0) base.OnClick();
        }
	}
}
