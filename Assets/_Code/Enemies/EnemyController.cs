using UnityEngine;

public  class EnemyController : MonoBehaviour {
    [SerializeField] protected float health;
    [SerializeField] protected float defence;

    protected float Positive(float value) {
        return Mathf.Clamp(value, 0, Mathf.Infinity);
    }

    public void TakeDamage(float damage, float armourPenetration = 0) {
        float defenceDmgReduction = Positive(defence - armourPenetration) / 2;
        damage -= defenceDmgReduction;
        health -= Positive(damage);
        Debug.LogWarning($"Health {health}");
    }
}