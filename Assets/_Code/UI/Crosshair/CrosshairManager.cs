using Globals;
using MathsAndSome;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Crosshair;

public class CrosshairManager : MonoBehaviour
{
    Player.PL_Controller pc;
    Image image;

    [Header("Images")]

    [SerializeField] Sprite defaultImage;
    [SerializeField] Sprite enemyImage;

    void Start() {
        pc = mas.player.Player;
        image = GetComponent<Image>();
    }

    void Update() {
        if (Physics.Raycast(pc.playerCamera.transform.position, pc.playerCamera.transform.forward, out RaycastHit hit, 20f)) {
            switch (hit.collider.tag) {
                //TODO: Fix this naming like what?
                case glob.enemyTag:
                    image.sprite = Resources.Load<Sprite>("EnemyCursor");
                    break;
                default:
                    image.sprite = Resources.Load<Sprite>("default_cursor");
                    break;
            }
        }

        else {
            image.sprite = Resources.Load<Sprite>("default_cursor");
        }
    }
}