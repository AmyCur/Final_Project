using UnityEngine;
using Cur.IO;
using System.Collections.Generic;

namespace Cur.Translation{
	public class MasterTranslation : MonoBehaviour{
		void Start(){
			string fileContent = (string)Read.ReadFile(GetTranslations.path);
			List<Text> texts = Cur.JSON.Deserialization.Deserialize<Text>(fileContent);
			if(texts.Count>0){
				foreach(Text t in texts){
					Debug.Log(t.english);
				}
			}
			else{
				Debug.Log($"file Content: {fileContent} ||| texts : {string.Join(' ',texts)}");
			}
		}
		
	}
}