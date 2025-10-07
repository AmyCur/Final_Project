using UnityEngine;
using Elements;
using MathsAndSome;

public abstract class ProjectileController : MonoBehaviour {

    [Header("Damage")]
    protected float damage;
    protected Element element;

    protected Vector3 GetPlayerVector() {
        Vector3 player = mas.player.GetPlayer().transform.position;
        Vector3 delta = (transform.position - player).normalized;

        return delta;
    }

    protected virtual void Start() { }

    protected virtual void Update() { }
    
    protected virtual void DestroyObject(){ Destroy(gameObject); }
}