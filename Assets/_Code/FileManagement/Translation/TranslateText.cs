using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Cur.Translation{
	public class TranslateText : MonoBehaviour{
		
		static TaskCompletionSource<bool> textLoaded = new TaskCompletionSource<bool>();

		async Task SetText(){
			await textLoaded.Task;
			
		}
		
		void Start() {
			
		}
	}
}