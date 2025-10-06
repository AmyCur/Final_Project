using Elements;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityLib
{
    public static class Entity
    {
        public static bool isEntity(this object m) {
            if (m is RaycastHit h) return !!(h.collider.GetComponent<ENT_Controller>()) || h.collider.CompareTag(Globals.glob.playerChildTag);
            else if (m is Collider c) return !!(c.GetComponent<ENT_Controller>()) || c.CompareTag(Globals.glob.playerChildTag);
            else if (m is MonoBehaviour mono) return mono is ENT_Controller;

            return false;
        }

        public static bool isPlayer(this object m)
        {
            if (m is RaycastHit h) return !!h.collider.GetComponent<PL_Controller>();
            else if (m is Collider c) return !!(c.GetComponent<PL_Controller>()) || c.CompareTag(Globals.glob.playerChildTag);
            else if (m is MonoBehaviour mono) return mono is PL_Controller;
            return false;
        }

        public static bool isEnemy(this object m)
        {
            if (m is RaycastHit h) return h.collider.tag == Globals.glob.enemyTag || !!h.collider.GetComponent<ENM_Controller>();
            else if (m is Collider c) return !!c.GetComponent<ENM_Controller>();
            else if (m is MonoBehaviour mono) return mono is ENM_Controller;
            return false;
        }

        public static bool inRange(this float v, float minRange, float maxRange)
        {
            return v >= minRange && v < maxRange;
        }
    }
}