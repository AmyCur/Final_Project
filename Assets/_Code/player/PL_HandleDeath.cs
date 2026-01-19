using Magical;
using MathsAndSome;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player.Movement;

namespace Player{
	public class PL_HandleDeath : MonoBehaviour {
		PL_Controller pc;

		void Start() {
			pc=mas.player.Player;
		}

		void Update(){
			if(pc.state == PlayerState.dead){
				if(magic.key.down(keys.jump)){
					SceneManager.LoadScene(0);
				}
			}
		}
	}
}