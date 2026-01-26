using Globals;
using Magical;
using MathsAndSome;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Combat.Attacks;
using System.Threading.Tasks;
using Audio;
using Player.Movement;

namespace Combat {
	[System.Serializable]
	public class SingleAttack : IEnumerable<SingularAttack>{
		public SingularAttack primary;
		public AlternateAttack assist;
		public AlternateAttack ability;
		
		public IEnumerator<SingularAttack> GetEnumerator(){
			yield return primary;
			yield return assist;
			yield return ability;
		}
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class CombatController : MonoBehaviour {
		public SingleAttack ca;
		public List<SingleAttack> attacks;
		public int caIndex;

		[Header("Admin")]

		public SingleAttack spawner;

		void CheckSwitchWeapon(){
			int currentKey = (int)magic.key.PressedKey();
			//* 49=1 key //*57=9 key

			if(magic.key.down(KeyCode.Alpha5) && mas.player.Player.admin){
				ca=spawner;
			}
			
			else if(currentKey >= 49 && currentKey <= 57){
				if(attacks[currentKey-49].primary != null) {
					caIndex=currentKey-49;
					ca=attacks[caIndex];
				}				
			}

			
		}

		void CheckForAttackPressed(){
			// Help idk if this is good or not am i doing it wrong :scared:
			foreach(SingularAttack atk in ca){
				if (atk.canAttack && atk.keyDown()){
					atk.OnClick();
				}
			}
		}

		void OnValidate() {
			if(caIndex<attacks.Count){
				ca=attacks[caIndex];
			}
		}

		void Start(){
			ca=attacks[caIndex];
			(spawner.primary as SpawnerController).Start();

			foreach(SingleAttack atk in attacks){
				foreach(SingularAttack sAtk in atk){
					sAtk.canAttack=true;
				}
			}
		}

		void Update(){
			CheckSwitchWeapon();
			CheckForAttackPressed();
		}

		public void OnDrawGizmos(){
			PL_Controller pc = mas.player.Player;
			if(pc!=null && pc.playerCamera != null){
				Gizmos.DrawCube(pc.transform.position+(pc.playerCamera.transform.forward*2f), mas.vector.MultiplyVectors(new List<Vector3>(){new Vector3(1.45f,1.45f,1.45f), Vector3.one}));		
			}
		}
	}

}