using Globals;
using MathsAndSome;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    PlayerController pc;
    Image image;

    [Header("Images")]

    [SerializeField] Sprite defaultImage;
    [SerializeField] Sprite enemyImage;


    void Start() {
        pc = mas.player.GetPlayer();
        image = GetComponent<Image>();
    }

    void Update() {
        if (Physics.Raycast(pc.playerCamera.transform.position, pc.playerCamera.transform.forward, out RaycastHit hit, 20f)) {
            switch (hit.collider.tag) {
                case glob.enemyTag:
                    image.sprite = Resources.Load<Sprite>("EnemyCursor");
                    Debug.Log("enemy");
                    break;
                default:
                    image.sprite = Resources.Load<Sprite>("default_cursor");
                    Debug.Log("default");
                    break;
            }
        }

        else {
            image.sprite = Resources.Load<Sprite>("default_cursor");
            Debug.Log("default but other");
        }


        

        
    }
}