using UnityEngine;

namespace UI{
	public static class MainMenuManager{
		public static GameObject page;
		public static GameObject settingsPage;

		[RuntimeInitializeOnLoadMethod]
		static void Start()
		{
			page=GameObject.Find("TitlePage");
		}
	}
}