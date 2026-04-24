
using MathsAndSome;
using System.Collections.Generic;

namespace UI{

public static class HandleAllMenus{
	public static List<HandleScreen> openScreens = new();

	public static void CloseOpenScreens(){
		if(openScreens.Count>0){
			foreach(HandleScreen hc in openScreens){
				if(hc.open){
					mas.player.Player.StartCoroutine(hc.CloseScreen());
				}
			}
		}
		
	}
}}