using UnityEngine;
using Elements;
using MathsAndSome;

public class ProjectileController : MonoBehaviour {

	public enum Target {
		player,
		enemy,
		both
	}

	[Header("Damage")]
	[SerializeField] protected float damage;
	[SerializeField] protected Element element;
	[SerializeField] protected Target target;
	[SerializeField] protected bool homing;

	protected Vector3 GetPlayerVector() {
		Vector3 player = mas.player.GetPlayer().transform.position;
		Vector3 delta = (transform.position - player).normalized;

		return delta;
	}

	protected virtual void Start() { }

	protected virtual void Update() { }

	protected virtual void DestroyObject() { Destroy(gameObject); }
}