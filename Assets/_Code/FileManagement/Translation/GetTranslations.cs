using System.Collections.Generic;
using System.IO;
using Cur.IO;
using UnityEngine;

namespace Cur.Translation{

	[System.Serializable]
	public class Text{
		public string english;
		public string spanish;
		public string german;
		public string french;

		public static Text CreateFromJSON(string jsonString){
			return JsonUtility.FromJson<Text>(jsonString);
		}
	}

	
	
	public static class GetTranslations{
		public static string path => Application.persistentDataPath + "/Translation/Translations.json";

		

	}
}