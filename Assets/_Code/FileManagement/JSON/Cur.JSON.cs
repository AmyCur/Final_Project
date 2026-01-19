using Cur.Translation;
using System.Collections.Generic;
using UnityEngine;

namespace Cur.JSON{
	


	public static class Serialization{
	}
	
	public static class Deserialization{
		public static T Deserialize<T>(string fileContents){
			return JsonUtility.FromJson<T>(fileContents);
		}

	}
}