using UnityEngine;
using Globals;

public class RaincloudController : MonoBehaviour
{
    public float moveSpeed=12f;
	public float searchDistance=30f;
	GameObject targetEnemy;

	GameObject GetNearestEnemy(){
		GameObject[] enemies = GameObject.FindGameObjectsWithTag(glob.enemyTag);
		GameObject nearest=null;
		float distance=10_000f;

		foreach(GameObject obj in enemies){
			if(
				Physics.Raycast(transform.position, (obj.transform.position-transform.position).normalized, out RaycastHit hit, searchDistance)){
				if(hit.distance<distance){
					distance=hit.distance;
					nearest=obj;
				}
			}
		}

		return nearest;
	}

	long tick;

	void Update(){
		tick+=1;
		if(tick>=250){
			tick=0;
			targetEnemy=GetNearestEnemy();
			Debug.Log(targetEnemy?.name);
		}
	}
}