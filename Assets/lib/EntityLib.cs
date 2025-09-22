using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityLib
{
    public static class Entity
    {
        public static bool isEntity(this object m) {
            if (m is RaycastHit h) return (h.collider.GetComponent<EntityController>() != null) || h.collider.CompareTag(Globals.glob.playerChildTag);
            else if (m is Collider c) return (c.GetComponent<EntityController>() != null) || c.CompareTag(Globals.glob.playerChildTag);
            else if (m is MonoBehaviour mono) return mono is EntityController;

            return false;
        }

        public static bool isPlayer(this object m)
        {
            if (m is RaycastHit h) return h.collider.GetComponent<PlayerController>() != null;
            else if (m is Collider c) return (c.GetComponent<PlayerController>() != null) || c.CompareTag(Globals.glob.playerChildTag);
            else if (m is MonoBehaviour mono) return mono is PlayerController;
            return false;
        }

        public static bool isEnemy(this object m)
        {
            if (m is RaycastHit h) return h.collider.GetComponent<EnemyController>() != null;
            else if (m is Collider c) return c.GetComponent<EnemyController>() != null;
            else if (m is MonoBehaviour mono) return mono is EnemyController;
            return false;
        }

        public static bool inRange(this float v, float minRange, float maxRange)
        {
            return v >= minRange && v < maxRange;
        }
    }
}