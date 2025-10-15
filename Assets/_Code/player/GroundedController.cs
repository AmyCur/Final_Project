using UnityEngine;
using static EntityLib.Entity; 

public class GroundedController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.isEntity())
        {
            Debug.Log("Something");
        }
    }
}
