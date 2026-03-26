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
				string[] splitCPP = PlayerPrefs.GetString(CPPKey).Split("|")[0].Split(":");

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
				string[] splitForard = PlayerPrefs.GetString(CPPKey).Split("|")[1].Split(":");
				Vector3 fwVector=new Vector3(
					float.Parse(splitForard[0]),
					float.Parse(splitForard[1]),
					float.Parse(splitForard[2])
			
				);
				mas.player.Player.transform.localRotation=Quaternion.LookRotation(fwVector, Vector3.up);
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
			Vector3 CPPR = currentCP.transform.forward;
			PlayerPrefs.SetString(CPPKey, $"{CPP.x}:{CPP.y}:{CPP.z}|{CPPR.x}{CPPR.y}{CPPR.z}");
			PlayerPrefs.Save();
		}
	}
}