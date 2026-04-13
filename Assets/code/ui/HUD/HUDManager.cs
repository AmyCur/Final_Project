using MathsAndSome;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD{
	public static class HUDManager{

		const string weaponIconTag="WeaponIcon";

		static Image weaponIcon => GameObject.FindGameObjectWithTag(weaponIconTag).GetComponent<Image>();
		
		public static void UpdateWeaponIcon(){
			weaponIcon.sprite=mas.player.Combat.ca.primary.weaponSprite;
		}
	}
}