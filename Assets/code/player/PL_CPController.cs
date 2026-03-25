using Combat;
using MathsAndSome;
using UnityEditor;
using UnityEngine;

namespace Player{
	public class PL_CPController : MonoBehaviour{
		public CP currentCP;
		public const string CPPKey = "currentCPPosition";

		public Vector3 ReadCurrentCPP {
			get {
				string[] splitCPP = PlayerPrefs.GetString(CPPKey).Split(":");

				return new Vector3(
					float.Parse(splitCPP[0]),
					float.Parse(splitCPP[1]),
					float.Parse(splitCPP[2])
				);
			}
		}

		void Start() {
			// If the current position cp has been set
		

			if(PlayerPrefs.HasKey(CPPKey)){
				mas.player.Player.transform.position=ReadCurrentCPP;
			}

			
		}

		void OnTriggerEnter(Collider other) {
			if(other.TryGetComponent<CP>(out CP cp)){
				currentCP=cp;
				SaveCP();
			}	
		}
		
		void SaveCP(){
			Vector3 CPP = currentCP.transform.position;
			PlayerPrefs.SetString(CPPKey, $"{CPP.x}:{CPP.y}:{CPP.z}");
			PlayerPrefs.Save();
		}
	}
}