using MathsAndSome;
using UnityEngine;

public class CollisionChecker : MonoBehaviour {

    Player.PL_Controller pc;

    void OnTriggerEnter(Collider other) {
        // if (!pc.justDashed) {
        //     pc.dashForceMultiplier = 0f;
        // }
    }

	void Start() {
        pc = mas.player.GetPlayer();
	}
}
