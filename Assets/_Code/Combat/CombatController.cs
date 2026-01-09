using Globals;
using Magical;
using MathsAndSome;
using System.Collections.Generic;
using UnityEngine;
using Combat.Attacks;
using System.Threading.Tasks;
using Cur.Audio;

namespace Combat {
	[System.Serializable]
	public class SingleAttack {
		public PrimaryAttack primary;
		public AlternateAttack assist;
		public AlternateAttack ability;
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
				currentKey-=48;
				caIndex=currentKey;
				if(attacks[currentKey-48].primary != null) {
					ca=attacks[caIndex-48];
				}
			}
		}

		void Update(){
			CheckSwitchWeapon();
		}
	}

}