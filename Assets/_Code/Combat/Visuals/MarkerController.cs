using UnityEngine;
using MathsAndSome;
using System.Collections;

public static class MarkerUtils {
    public const float markerDestructionTime = 2f;
}

public class MarkerController : MonoBehaviour {
    Transform player;
    SpriteRenderer sprite;

    public IEnumerator DestroyMarker() {
        yield return new WaitForSeconds(MarkerUtils.markerDestructionTime);
    }

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        player = mas.player.Player.gameObject.transform;
    }


    void SetPosition() {
        transform.localPosition = new Vector3
        (
            player.transform.position.x - transform.position.x,
            transform.localPosition.y,
            player.transform.position.z - transform.position.z
        ).normalized;
    }

    void SetRotation() => transform.LookAt(player);

    // If you get too close to the enemy, the marker goes crazy so this is a fix, plus it removes a bit of clutter
    // Still goes crazy when youre above though
    void SetOpacity() {

        float mag = Vector3.Magnitude(player.transform.position - transform.position) - 2f;

        sprite.color = new Color(
            255,
            255,
            255,
            Mathf.Clamp(mag, 0f, 1f)
        );
       
    }

    void Update() {
        SetPosition();
        SetRotation();
        SetOpacity();
    }
}
