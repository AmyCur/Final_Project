using MathsAndSome;
using UnityEngine;

namespace Player;

public class CollisionChecker : MonoBehaviour {

    Player.PL_Controller pc;

    void OnTriggerStay(Collider other) {
        if (!pc.shouldDash) {
            pc.dash.force = 0f;
        }
    }

	void Start() {
        pc = mas.player.Player;
	}
}
