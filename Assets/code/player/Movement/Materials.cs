using MathsAndSome;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Movement;

public static class Materials{
	public static PL_Controller pc => mas.player.Player;

	public class GameObjectTris{
		public GameObject obj;
		public int[] tris;

		public GameObjectTris(GameObject obj, int[] tris){
			this.obj=obj;
			this.tris=tris;
		}
	}

	public class GOTMaterial{
		public GameObjectTris got;
		public Material material;
	}

	public static List<GameObjectTris> levelTris;

	public static List<GameObjectTris> GetLevelTriangles(){

		List<GameObjectTris> gots = new();
		GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

		foreach(GameObject obj in allObjects){
			if(obj.TryGetComponent<Mesh>(out Mesh m)){
				gots.Add(new(obj, m.GetTriangles(0)));
			}
		}

		return gots;	
	}

	public static Material GetStoodMaterial(){
		if(levelTris==null) levelTris=GetLevelTriangles();

		if(Physics.Raycast(pc.transform.position, Vector3.down, out RaycastHit hit, pc.transform.localScale.y/2f)){
			foreach(GameObjectTris got in levelTris){
				if(got.obj == hit.collider.gameObject){
					
				}
			}
		}

		return null;
	}

	public class MaterialSound{
		public Material material;
		public AudioClip sound;

		public MaterialSound(Material material, AudioClip sound){
			this.material=material;
			this.sound=sound;
		}
	}
}
