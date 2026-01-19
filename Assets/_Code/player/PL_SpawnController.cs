using UnityEngine;
using MathsAndSome;
using System.Collections.Generic;
using Magical;
using Cur.UI;

namespace Player{
	public class PL_SpawnController : MonoBehaviour{
		public List<GameObject> cps;
		[SerializeField] int currentCPIndex=0;
		Vector3 positionBeforeMove=new Vector3(-999,-321,31211);
		
		void UpdatePlayerPos(){
			positionBeforeMove=transform.position;
			if(cps[currentCPIndex]!=null) {
				gameObject.transform.position = cps[currentCPIndex].transform.position;
				NotificationManager.AddNotification($"Teleported to {cps[currentCPIndex].name}");
			}
			
		}

		void Start() => UpdatePlayerPos();

		void HandlePosition(){ 
			if(mas.player.Player.admin){
				if(magic.key.down(KeyCode.N)){
					currentCPIndex++;
					if(currentCPIndex>=cps.Count) currentCPIndex=0;
				}

				else if(magic.key.down(KeyCode.P)){
					currentCPIndex--;
					if(currentCPIndex<0) currentCPIndex=cps.Count-1;
				}

				else if(magic.key.down(KeyCode.U) && positionBeforeMove != new Vector3(-999,-321,31211)){
					transform.position=positionBeforeMove;
				}
				
				else return;

				UpdatePlayerPos();
			}
		}

		void Update(){
			HandlePosition();
		}
	}
}