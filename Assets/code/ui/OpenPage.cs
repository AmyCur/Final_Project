using UnityEngine;
using System.Collections;
using MathsAndSome;

namespace UI{
	public enum PageTarget{
		page,
		settings_page
	}


	public class OpenPage : MonoBehaviour{
		public GameObject targetPage;
		public PageTarget pageTarget=PageTarget.page;
		Coroutine openPage;

		public IEnumerator LarpPage(Vector3 targetScale){
			if(targetScale.magnitude!=0) MainMenuManager.page.SetActive(true);
			MainMenuManager.page.transform.localScale = targetScale==Vector3.zero ? MainMenuManager.page.transform.localScale : Vector3.zero;
			while (targetPage.transform.localScale != targetScale){
				targetPage.transform.localScale=mas.vector.LerpVectors(targetPage.transform.localScale, targetScale, Time.deltaTime * 10f);
				yield return 0;
			}
			if(targetScale.magnitude==0) MainMenuManager.page.SetActive(false);
		}


		public void HandleOpenPage(){
			if(pageTarget==PageTarget.page){
				if(MainMenuManager.page!=null) MainMenuManager.page.SetActive(false);
				if(targetPage!=MainMenuManager.page){
					MainMenuManager.page=targetPage;
					StartCoroutine(LarpPage(new(1,1,1)));
				}
				else{
					MainMenuManager.page.SetActive(!MainMenuManager.page.activeInHierarchy);
				}
			
			}

			else{
				if(MainMenuManager.settingsPage!=null) MainMenuManager.settingsPage.SetActive(false);
				if(targetPage!=MainMenuManager.settingsPage){
					MainMenuManager.settingsPage=targetPage;
					StartCoroutine(LarpPage(Vector3.zero));
				}
				else{
					MainMenuManager.settingsPage.SetActive(!MainMenuManager.settingsPage.activeInHierarchy);
				}
			}
		}
	}
}