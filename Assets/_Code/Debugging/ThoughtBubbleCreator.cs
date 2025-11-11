using EntityLib;
using MathsAndSome;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThoughtBubbleCreator : MonoBehaviour {
    [Header("Display Settings")]
    public float displayRange = 10f;
    GameObject textMenu;

    GameObject player;

    void UpdateRadius() => GetComponent<SphereCollider>().radius = displayRange;
       
    void OnValidate() => UpdateRadius();

    void Start()
    {
        UpdateRadius();
        player = mas.player.GetPlayer().gameObject;
        textMenu = Resources.Load<GameObject>("Prefabs/Debugging/TextMenu");
    }

    void OnTriggerEnter(Collider other) {
        List<GameObject> currentBubbles = GameObject.FindGameObjectsWithTag("Thought").ToList();

        if(Settings.Settings.Development.displayThoughts){
            if (other.isEntity(typeof(ENM_Controller))) {
                bool contains = false;
                for (int i = 0; i < other.transform.childCount - 1; i++) {
                    if (currentBubbles.Contains(other.transform.GetChild(i).gameObject)) contains = true;
                }

                if (!contains) {
                    GameObject bu = Instantiate(textMenu, other.transform.position, Quaternion.identity);
                    bu.transform.SetParent(other.transform);
                    bu.transform.position = new(bu.transform.position.x, bu.transform.position.y + other.transform.localScale.y*1.1f, bu.transform.position.z);
                    ThoughtBubble tb = bu.GetComponent<ThoughtBubble>();
                    tb.SetText(other.GetComponent<RB_Controller>().thoughts);
                }
            }
        }
    }

	void OnTriggerExit(Collider other) {
        if (other.isEntity(typeof(ENM_Controller)))
        {
            for (int i = 0; i < other.transform.childCount; i++)
            {
                if (other.transform.GetChild(i).CompareTag("Thought")) Destroy(other.transform.GetChild(i).gameObject);
            }
        }
	}


	void Update() {
        transform.position = player.transform.position;
    }
}