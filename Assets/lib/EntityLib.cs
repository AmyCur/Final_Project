using UnityEngine;

namespace EntityLib
{
    public static class Entity
    {
        public static bool isEntity(this object m)
        {
            if (m is RaycastHit h) return h.collider.GetComponent<EntityController>() != null;
            else if (m is MonoBehaviour mono) return mono is EntityController || mono.gameObject.tag==Globals.glob.playerTag;
            return false;
        }

        public static bool isPlayer(this object m)
        {
            if (m is RaycastHit h) return h.collider.GetComponent<PlayerController>() != null;
            else if (m is MonoBehaviour mono) return mono is PlayerController;
            return false;
        }

        public static bool isEnemy(this object m)
        {
            if (m is RaycastHit h) return h.collider.GetComponent<EnemyController>() != null;
            else if (m is MonoBehaviour mono) return mono is EnemyController;
            return false;
        }

        public static bool inRange(this float v, float minRange, float maxRange)
        {
            return v >= minRange && v < maxRange;
        }
    }
}