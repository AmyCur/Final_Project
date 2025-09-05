using UnityEngine;
using System.Collections.Generic;
using System;

namespace VectorUtils
{
    public static class VU
    {
        public static Vector3 GetRandomVectorInRadius(Vector3 v, float radius)
        {
            float x = UnityEngine.Random.Range(0.1f, 1f) * radius;
            float z = UnityEngine.Random.Range(0.1f, 1f) * radius;
            return new(v.x + x, v.y, v.z + z);
        }

        public static Vector3 LerpVectors(Vector3 v1, Vector3 v2, float a)
        {
            return new Vector3(
              Mathf.Lerp(v1.x, v2.x, a),
              Mathf.Lerp(v1.y, v2.y, a),
              Mathf.Lerp(v1.z, v2.z, a)
            );
        }

        public static Vector3 Abs(Vector3 v3)
        {
            // If the vector is already all positive, there is no need to compute the abs of it
            if (v3.x > 0 && v3.y > 0 && v3.z > 0)
            {
                return v3;
            }

            return new Vector3(Math.Abs(v3.x), Math.Abs(v3.y), Math.Abs(v3.z));
        }

        public static Vector3 Multiply(List<Vector3> vectors)
        {
            Vector3 v3 = vectors[0];
            for (int i = 0; i < vectors.Count; i++)
            {
                if (i != 0)
                {
                    v3 = new Vector3(v3.x * vectors[i].x, v3.y * vectors[i].y, v3.z * vectors[i].z);
                }
            }
            return v3;
        }
        //Physics
        // This divides n vectors where 0 < n <= infinity
        public static Vector3 Divide(List<Vector3> vectors)
        {
            Vector3 v3 = vectors[0];
            for (int i = 0; i < vectors.Count; i++)
            {
                if (i != 0)
                {
                    v3 = new Vector3(v3.x / vectors[i].x, v3.y / vectors[i].y, v3.z / vectors[i].z);
                }
            }
            return v3;
        }

        public static Quaternion ToQuaternion(Vector3 v3)
        {
            return Quaternion.Euler(v3.x, v3.y, v3.z);
        }

        public static Vector3 ClampVector(Vector3 vectorToClamp, Vector3[] clamp)
        {
            float x = Mathf.Clamp(vectorToClamp.x, clamp[0].x, clamp[1].x);
            float y = Mathf.Clamp(vectorToClamp.y, clamp[0].y, clamp[1].y);
            float z = Mathf.Clamp(vectorToClamp.z, clamp[0].z, clamp[1].z);

            return new Vector3(x, y, z);
        }

    }
}
