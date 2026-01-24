using MathsAndSome;
using Player.Movement;
using System;
using TMPro;
using UnityEngine;


namespace FileManagement.UI{
	public class HealthPopup : Popup{
		PL_Controller pc;
		TMP_Text message;
		bool active=false;

		char[] characters = {'!', '£', '$', '€', '%', '^', '&', '*', '(', ')', '-', '_', '+', '='};

		public override void Start(){
			base.Start();
			pc=mas.player.Player;
			message=transform.GetChild(0).GetComponent<TMP_Text>();
		}

		

		void Update() {
			if(pc.health.h <= pc.health.maxHealth/2f){
				if(!active){
					if(sizeRoutine!=null) StopCoroutine(sizeRoutine);
					sizeRoutine = StartCoroutine(ChangeSize());
					active=true;
				}

				message.text=Jumble.DistortString("LOCK IN!", 30);
				if(pc.health.h <= pc.health.maxHealth/4f){
					message.text=Jumble.DistortString("YOURE TAKING TOO MUCH DAMAGE!");

					if(pc.health.h <= pc.health.maxHealth/10f){
					message.text=Jumble.DistortString("YOURe aisdj sdjwi ajiejdiasjdi djwia!", 1);
					}
				}
			}
			else{
				if(active) {
					if(sizeRoutine!=null) StopCoroutine(sizeRoutine);
					sizeRoutine = StartCoroutine(ChangeSize(false));
					active=false;
				}
			}
		}
	}
}