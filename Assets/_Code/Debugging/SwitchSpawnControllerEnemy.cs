using Combat;
using MathsAndSome;
using UnityEngine;

public class SwitchSpawnControllerEnemy : MonoBehaviour
{
    SpawnerController sc;
    public Enemy enm;

    public void SetEnemy() => sc.enemy=enm;

	void Start() {
        sc=mas.player.GetPlayerObj().GetComponent<CombatController>().spawner.primary as SpawnerController;
	}
}
