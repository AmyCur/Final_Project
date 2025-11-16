using System;
using System.Collections.Generic;
using Globals;
using UnityEngine;

namespace MathsAndSome
{
    public static class mas
    {
        public static class get {
            public static Transform VortexHolder() {
                return GameObject.FindGameObjectWithTag("VortexHolder").transform;
            }

            public static Cur.UI.HUDController HC() {
                return GameObject.FindGameObjectWithTag(glob.WeaponIconTag).GetComponent<Cur.UI.HUDController>();
            }
        }


        public static class vector {
            public static Vector3 GetRandomVectorInRadius(Vector3 v, float radius) {
                float x = UnityEngine.Random.Range(0.1f, 1f) * radius;
                float z = UnityEngine.Random.Range(0.1f, 1f) * radius;
                return new(v.x + x, v.y, v.z + z);
            }

            public static Vector3 ClampVectorWithFloat(Vector3 v1, float min, float max) {
                return new(Mathf.Clamp(v1.x, min, max), Mathf.Clamp(v1.y, min, max), Mathf.Clamp(v1.z, min, max));
            }

            // This takes two Vector 3s and a float t as an input and returns the linear interpolation of the two vectors at (t*100)% 
            public static Vector3 LerpVectors(Vector3 v1, Vector3 v2, float a) {
                return new Vector3(
                    Mathf.Lerp(v1.x, v2.x, a),
                    Mathf.Lerp(v1.y, v2.y, a),
                    Mathf.Lerp(v1.z, v2.z, a)
                );
            }

            public static Vector3 SubVectors(List<Vector3> v3s) {
                Vector3 vector = v3s[0];
                // Skip root
                for (int i = 1; i < v3s.Count; i++) {
                    vector = new Vector3(vector.x - v3s[i].x, vector.y - v3s[i].y, vector.z - v3s[i].z);
                }

                return vector;
            }

            // This returns the absolute value of a vector (All numbers are positive)
            public static Vector3 AbsVector(Vector3 v3) {
                // If the vector is already all positive, there is no need to compute the abs of it
                if (v3.x > 0 && v3.y > 0 && v3.z > 0) {
                    return v3;
                }

                return new Vector3(Math.Abs(v3.x), Math.Abs(v3.y), Math.Abs(v3.z));
            }

            // This multiplies n vectors where 0 < n <= infinity
            public static Vector3 MultiplyVectors(List<Vector3> vectors) {
                Vector3 v3 = vectors[0];
                for (int i = 0; i < vectors.Count; i++) {
                    if (i != 0) {
                        v3 = new Vector3(v3.x * vectors[i].x, v3.y * vectors[i].y, v3.z * vectors[i].z);
                    }
                }
                return v3;
            }

            // This divides n vectors where 0 < n <= infinity
            public static Vector3 DivideVectors(List<Vector3> vectors) {
                Vector3 v3 = vectors[0];
                for (int i = 0; i < vectors.Count; i++) {
                    if (i != 0) {
                        v3 = new Vector3(v3.x / vectors[i].x, v3.y / vectors[i].y, v3.z / vectors[i].z);
                    }
                }
                return v3;
            }

            public static Vector3 InvertVector(Vector3 vector) {
                return -vector;
            }

            public static Quaternion v3q(Vector3 v3) {
                return Quaternion.Euler(v3.x, v3.y, v3.z);
            }

            public static float AddVectorComponents(Vector3 v) {
                return v.x + v.y + v.z;
            }

            public static Quaternion VectorsToQuaternion(Vector3 v1, Vector3 v2) {

                Vector3 v3 = new Vector3
                (1 / Mathf.Sin((v1.y - v2.y) / (v1.x - v2.x)),
                1 / Mathf.Sin((v1.z - v2.z) / (v1.y - v2.y)),
                0);

                return Quaternion.Euler(v3);
            }
            public static Vector3 zeroY(Vector3 v) {
                return new Vector3(v.x, 0, v.z);
            }

            public static Vector3 ClampVector(Vector3 vectorToClamp, Vector3[] clamp) {
                float x = Mathf.Clamp(vectorToClamp.x, clamp[0].x, clamp[1].x);
                float y = Mathf.Clamp(vectorToClamp.y, clamp[0].y, clamp[1].y);
                float z = Mathf.Clamp(vectorToClamp.z, clamp[0].z, clamp[1].z);

                return new Vector3(x, y, z);
            }
        }

        // This detects whether a target object is within a radius around a vector 3 point
        public static bool isInRadiusToPoint(Vector3 inputPosition, Vector3 targetObj, string targetTag, float range)
        {

            /*
                This needs to check to see if inputPosition is around "point" at a distance <= "range"           
            */

            if (Physics.Raycast(inputPosition, targetObj - inputPosition, out RaycastHit hit, range))
            {
                if (hit.collider.tag == targetTag)
                {
                    return true;
                }
            }

            // Vector2 direction = new Vector2(-1, -1);

            // // This scans through EVERY possible angle on 0.1f increments
            // while(direction.x < 1){
            //     direction = new Vector2(direction.x + 0.1f, -1);
            //     while(direction.y < 1){
            //         direction = new Vector2(direction.x, direction.y + 0.1f);

            //         if(Physics.Raycast(inputPosition, new Vector2(direction.x, direction.y), out RaycastHit hit, range)){
            //             if(hit.collider.tag == targetTag){
            //                 Debug.Log("Truth Nuke!");
            //                 return true;  
            //             }
            //         }
            //     }
            // }

            return false;

        }

        public static class normal
        {
            public static Vector3 GetNormalFromListOfColliders(List<Collider> colliders)
            {
                Vector3 normal = Vector3.zero;

                foreach (Collider col in colliders)
                {
                    if (normal.x != 1)
                    {
                        normal += new Vector3(-col.transform.right.x, 0, 0);
                    }

                    if (normal.y != 1)
                    {
                        normal += new Vector3(0, -col.transform.right.y, 0);
                    }

                    if (normal.z != 1)
                    {
                        normal += new Vector3(0, 0, -col.transform.right.z);
                    }

                }

                return normal;
            }
            public static Vector3 INormal(Collider col)
            {
                return -col.transform.forward;
            }
        }

        public static class player
        {
            public static Vector3 PlayerDistance(GameObject player, GameObject gameObject)
            {
                return vector.AbsVector(player.transform.position - gameObject.transform.position);
            }

            public static GameObject GetPlayerObj() => GameObject.FindGameObjectWithTag(glob.playerTag);
            
            public static Player.PL_Controller GetPlayer() => GetPlayerObj().GetComponent<Player.PL_Controller>();
            

            // public static Combat.CombatController GetCombat.CombatController()
            // {
            //     return GameObject.FindGameObjectWithTag(glob.playerTag).GetComponent<Combat.CombatController>();
            // }

            public static RaycastHit ShootPlayer(GameObject player, Vector3 origin)
            {
                if (Physics.Raycast(origin, player.transform.position - origin, out RaycastHit hit, glob.maxAiCheckRange))
                {
                    return hit;
                }

                // If this fails then hit.collider will be null
                return new RaycastHit();
            }

            public static List<Collider> RemovePlayerFromList(List<Collider> list)
            {
                list.Remove(GetPlayer().GetComponent<CapsuleCollider>());
                return list;
            }

            // public static Vector3 PlayerForward()
            // {
            //     return GetPlayer().forwardObject.transform.forward;
            // }

            // public static Mon GetMono()
            // {
            //     return GameObject.FindGameObjectWithTag(glob.playerTag).GetComponent<Mon>();

            // }
        }

        public static Collider[] GetCollidersInArea(Transform transform)
        {
            return Physics.OverlapBox
            (
                transform.position - (transform.localScale / 2),
                new Vector3(transform.localScale.x * 1.1f, transform.localScale.y, transform.localScale.z * 1.1f)
            );
        }

        public static class maths
        {
            public static float CalculateHypotenuse(Vector2 v)
            {
                return Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2));
            }

            public static float FindDotProduct(Vector2 a, Vector2 b)
            {

                // Get the length of those vectors
                float al = Mathf.Abs(CalculateHypotenuse(a));
                float bl = Mathf.Abs(CalculateHypotenuse(b));

                float d = al * bl;

                // Calculate the dotproduct
                float dp = (a.x * b.x) + (a.y * b.y);

                // Calculate the angle
                return 1 / Mathf.Cos(dp / d);

                // This uses this formula
                // \theta=\cos^{-1}\left(\frac{\frac{\left(a_{x}\cdot b_{x}\right)+\left(a_{y}\cdot b_{y}\right)}{\left|b\right|}}{\left|a\right|}\right)

            }
        }
    }

}


