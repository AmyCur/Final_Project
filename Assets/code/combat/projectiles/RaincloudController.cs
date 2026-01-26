using UnityEngine;
using Globals;
using System.Collections;
using Combat.Elements;
using Combat.Enemies;

[RequireComponent(typeof(Rigidbody))]
public class RaincloudController : MonoBehaviour
{
    public float moveSpeed=12f;
	public float searchDistance=30f;
	public float lifeTime=12f;
	public float hoverHeight = 10f;
	public float damage = 10f;
	public long actionSpeed=100;
	public ElementType element;
	Rigidbody  rb;
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

	IEnumerator DestroySelf(){
		yield return new WaitForSeconds(lifeTime);
		Destroy(gameObject);
	}

	void Start(){
		rb=GetComponent<Rigidbody>();
		StartCoroutine(DestroySelf());	
	}

	void OnTriggerStay(Collider other){
		if(other.TryGetComponent<ENM_Controller>(out ENM_Controller enm)){
			enm.TakeDamage(damage, new(element));
		}
	}

	void Update(){
		tick+=1;
		if(tick>=actionSpeed){
			tick=0;
			targetEnemy=GetNearestEnemy();
			Debug.Log(targetEnemy?.name);
		}

		Vector3 targetDirection=Vector3.zero;
		if(targetEnemy!=null) targetDirection = (targetEnemy.transform.position-transform.position).normalized;

		rb.AddForce(new Vector3(targetDirection.x,0f,targetDirection.z)*moveSpeed);
	}
}