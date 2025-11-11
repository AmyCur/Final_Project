using Elements;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityLib
{
    public static class Entity
    {
        // public static bool isEntity(this object m) {
        //     if (m is RaycastHit h) return !!(h.collider.GetComponent<ENT_Controller>()) || h.collider.CompareTag(Globals.glob.playerChildTag);
        //     else if (m is Collider c) return !!(c.GetComponent<ENT_Controller>()) || c.CompareTag(Globals.glob.playerChildTag);
        //     else if (m is MonoBehaviour mono) return mono is ENT_Controller;

        //     return false;
        // }

        // public static bool isPlayer(this object m)
        // {
        //     if (m is RaycastHit h) {
        //         return h.collider.CompareTag(Globals.glob.playerChildTag) || !!h.collider.GetComponent<Player.PL_Controller>() || h.collider.CompareTag(Globals.glob.playerTag );
        //     }
        //     else if (m is Collider c) return !!(c.GetComponent<Player.PL_Controller>()) || c.CompareTag(Globals.glob.playerChildTag);
        //     else if (m is MonoBehaviour mono) return mono is Player.PL_Controller;
        //     return false;
        // }

        // public static bool isEnemy(this object m) {
        //     if (m is RaycastHit h) return h.collider.tag == Globals.glob.enemyTag || !!h.collider.GetComponent<ENM_Controller>();
        //     else if (m is Collider c) return !!c.GetComponent<ENM_Controller>();
        //     else if (m is MonoBehaviour mono) return mono is ENM_Controller;
        //     return false;
        // }

        // static Dictionary<Type, string[]>

        static Dictionary<Type, string> enemyTypeToName = new() {
            {typeof(Player.PL_Controller), "player"},
            {typeof(ENM_Controller), "enemy"},
            {typeof(ENT_Controller), "entity"},
        };
        
        // This is so ugly and i hate it but i actually cant figure out another way because i am stupid
        public static bool isEntity(this object m, Type targetType=null) {
            targetType ??= typeof(ENT_Controller);           
           
            if (targetType == typeof(Player.PL_Controller)) {
                if (m is RaycastHit h) {
                    return h.collider.CompareTag(Globals.glob.playerChildTag) || !!h.collider.GetComponent<Player.PL_Controller>() || h.collider.CompareTag(Globals.glob.playerTag);
                }
                else if (m is Collider c) return (c.GetComponent<Player.PL_Controller>()) != null || c.CompareTag(Globals.glob.playerChildTag);
                else if (m is MonoBehaviour mono) return mono is Player.PL_Controller;
                return false;
            }

            else if (targetType == typeof(ENM_Controller)) {
                if (m is RaycastHit h) return h.collider.tag == Globals.glob.enemyTag || !!h.collider.GetComponent<ENM_Controller>();
                else if (m is Collider c) return !!c.GetComponent<ENM_Controller>();
                else if (m is MonoBehaviour mono) return mono is ENM_Controller;
                return false;
            }

            else if (targetType == typeof(ENT_Controller)) {
                if (m is RaycastHit h) return !!(h.collider.GetComponent<ENT_Controller>()) || h.collider.CompareTag(Globals.glob.playerChildTag);
                else if (m is Collider c) return !!(c.GetComponent<ENT_Controller>()) || c.CompareTag(Globals.glob.playerChildTag);
                else if (m is MonoBehaviour mono) return mono is ENT_Controller;
                return false;
            }

            return false;
        }

        public static bool inRange(this float v, float minRange, float maxRange)
        {
            return v >= minRange && v < maxRange;
        }
    }
}