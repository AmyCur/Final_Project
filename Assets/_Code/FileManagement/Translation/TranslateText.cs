using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace FileManagement.Translation;

public static class TranslateText{
	public static bool translated=false;
	public static TextList tl;
	
	public static Coroutine waitForLoadedRoutine;
	
	public static IEnumerator WaitForLoaded(){
		yield return new WaitUntil(() => true);
		translated=true;
	}
	
}
