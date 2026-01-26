using UnityEngine;


namespace Combat.Enemy {
   
    public enum Projectile {
        bullet,
        pipe
    }

    public enum ShotType {
        single,
        burst
    }

    [CreateAssetMenu(fileName = "New Attack Data", menuName = "Attacks/Create/Attack Data/New")]
    public class AttackData : ScriptableObject {

        [Header("Attack Stats")]

        public float damage=10f;
        public float attackCD=0.1f;
        public float attackRange=10f;
    }

    [CreateAssetMenu(fileName = "New Ranged Data", menuName = "Attacks/Create/Attack Data/Ranged")]
    public class RangedData : AttackData {
        

        [Header("Attack Data")]

        public ShotType shotType = ShotType.single;
        public Projectile projectile = Projectile.bullet;

        [Header("Burst")]
        [Min(1)] public int bulletsPerShot;
        public float timeBetweenBurstShot = 0.1f;
    }
}
