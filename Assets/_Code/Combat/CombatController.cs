using Globals;
using Magical;
using MathsAndSome;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Combat.Attacks;
using System.Threading.Tasks;
using Cur.Audio;

namespace Combat {
	[System.Serializable]
	public class SingleAttack : IEnumerable<SingularAttack>{
		public PrimaryAttack primary;
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
			if(currentKey >= 49 && currentKey <= 57){
				if(attacks[currentKey-49].primary != null) {
					caIndex=currentKey-49;
					print(currentKey-49);
					ca=attacks[caIndex];
				}
			}
		}

		void CheckForAttackPressed(){
			foreach(SingularAttack atk in ca){
				if (atk.canAttack && atk.keyDown()){
					atk.OnClick();
				}
			}
		}

		void Update(){
			CheckSwitchWeapon();
			CheckForAttackPressed();
		}
	}

}