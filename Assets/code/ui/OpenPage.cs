using UnityEngine;

namespace UI{
	public enum PageTarget{
		page,
		settings_page
	}


	public class OpenPage : MonoBehaviour{
		public GameObject targetPage;
		public PageTarget pageTarget=PageTarget.page;

		public void HandleOpenPage(){
			if(pageTarget==PageTarget.page){
				if(MainMenuManager.page!=null) MainMenuManager.page.SetActive(false);
				if(targetPage!=MainMenuManager.page){
					MainMenuManager.page=targetPage;
					MainMenuManager.page.SetActive(true);
				}
				else{
					MainMenuManager.page.SetActive(!MainMenuManager.page.activeInHierarchy);
				}
			
			}

			else{
				if(MainMenuManager.settingsPage!=null) MainMenuManager.settingsPage.SetActive(false);
				if(targetPage!=MainMenuManager.settingsPage){
					MainMenuManager.settingsPage=targetPage;
					MainMenuManager.settingsPage.SetActive(false);
				}
				else{
					MainMenuManager.settingsPage.SetActive(!MainMenuManager.settingsPage.activeInHierarchy);
				}
			}
		}

		void Start()
		{
		}
	}
}