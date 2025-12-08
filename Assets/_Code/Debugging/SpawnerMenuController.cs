using UnityEngine;
using MathsAndSome;

namespace Combat
{
	[CreateAssetMenu(fileName = "Spawner Controller", menuName = "Attacks/Create/Spawner Menu")]
	public class SpawnerMenuController : Attacks.AlternateAttack{

	   public bool menuActivated=false;

	   public override void OnClick(){
			mas.player.Player.EnemySpawnScreen.SetActive(!menuActivated);
			menuActivated=!menuActivated;
			Cursor.visible = menuActivated;
			Cur.UI.Props.inMenu=menuActivated;

			if(menuActivated)  Cursor.lockState = CursorLockMode.None;
			else  Cursor.lockState = CursorLockMode.Locked;
			// Debug.Log($"{mas.player.canva.name}");
	   }
   }
}