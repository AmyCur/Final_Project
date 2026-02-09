using UnityEngine;

public static class Instance<T> where T : MonoBehaviour{
	public static T instance
	{
		get{
			if(instance==null) return Object.FindFirstObjectByType(typeof(T)) as T; 
			return instance;
		}
		set{instance = value;}
	}
	
}