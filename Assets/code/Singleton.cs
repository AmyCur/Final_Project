using UnityEngine;


public class Singleton : MonoBehaviour{}
public class Singleton<T> : Singleton where T : Singleton<T>{
	public static T Instance {
		get{
			if(Instance==null) return Object.FindFirstObjectByType<T>();
			return Instance;
		} 
		set{
			Instance = value;
		}
	}
}

