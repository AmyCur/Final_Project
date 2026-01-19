using Combat;
using MathsAndSome;
using UnityEngine;
using TMPro;

public class SwitchSpawnControllerEnemy : MonoBehaviour
{
    SpawnerController sc;
    public Enemy enm;
	TMP_Text EnemyText => GameObject.Find("EnemyText").GetComponent<TMP_Text>();

    public void SetEnemy(){
		sc.enemy=enm;
		SetEnemyText();
	}

	void SetEnemyText() => EnemyText.text=$"Current enemy: {sc.enemy}";


	void Start() {
        sc=mas.player.GetPlayerObj().GetComponent<CombatController>().spawner.primary as SpawnerController;
		SetEnemyText();
	}
}