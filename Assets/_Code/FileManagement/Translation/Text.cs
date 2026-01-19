using Cur.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileManagement.Translation;

[System.Serializable]
public class Text{
	public string english;
	public string spanish;
	public string german;
	public string french;
}

[System.Serializable]
public class TextList : IEnumerable<Text>{
	public Text[] texts;

	public IEnumerator<Text> GetEnumerator(){
		for(int i = 0; i < texts.Length; i++){
			yield return texts[i];
		}
	}
	
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}