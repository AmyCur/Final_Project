using UnityEngine;
using MathsAndSome;
using System.Collections;

public static class MarkerUtils {
    public const float markerDestructionTime = 2f;
}

public class MarkerController : MonoBehaviour
{
    Transform player;

    public IEnumerator DestroyMarker()
    {
        yield return new WaitForSeconds(MarkerUtils.markerDestructionTime);
    }

    void Awake()
    {
        player = mas.player.GetPlayer().gameObject.transform;
    }


    void Update()
    {
        transform.LookAt(player);
    }
}
