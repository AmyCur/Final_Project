using MathsAndSome;
using Player;
using System;
using TMPro;
using UnityEngine;

namespace Cur.UI{
	public class HealthPopup : Popup{
		PL_Controller pc;
		TMP_Text message;
		System.Random rand;
		bool active=false;

		char[] characters = {'!', '£', '$', '€', '%', '^', '&', '*', '(', ')', '-', '_', '+', '='};

		public override void Start(){
			base.Start();
			pc=mas.player.Player;
			message=transform.GetChild(0).GetComponent<TMP_Text>();
			rand=new();
		}

		public char RandomLetter(){
			return characters[rand.Next(0, characters.Length)];
		}

		public string DistortString(string input, int corruptionRange=5){
			char[] working = input.ToCharArray();
			for(int i = 0; i < input.Length; i++){
				if(rand.Next(0,corruptionRange) == 0 && working[i] != ' '){
					working[i] = RandomLetter();
				}
			}
			return new string(working);
		}

		void Update() {
			if(pc.health.h <= pc.health.maxHealth/2f){
				if(!active){
					if(sizeRoutine!=null) StopCoroutine(sizeRoutine);
					sizeRoutine = StartCoroutine(ChangeSize());
					active=true;
				}

				message.text=DistortString("LOCK IN!", 30);
				if(pc.health.h <= pc.health.maxHealth/4f){
					message.text=DistortString("YOURE TAKING TOO MUCH DAMAGE!");

					if(pc.health.h <= pc.health.maxHealth/10f){
					message.text=DistortString("YOURe aisdj sdjwi ajiejdiasjdi djwia!", 1);
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