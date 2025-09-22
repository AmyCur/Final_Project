using EntityLib;
using MathsAndSome;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThoughtBubbleCreator : MonoBehaviour {
    [Header("Display Settings")]
    public float displayRange = 10f;
    public GameObject bubble;

    GameObject player;

    void Start() {
        player = mas.player.GetPlayer().gameObject;
    }

    void OnTriggerEnter(Collider other) {
        List<GameObject> currentBubbles = GameObject.FindGameObjectsWithTag("Thought").ToList();

        if (other.isEnemy()) {
            bool contains = false;
            for (int i = 0; i < other.transform.childCount - 1; i++) {
                if (currentBubbles.Contains(other.transform.GetChild(i).gameObject)) contains = true;
            }

            if (!contains) {
                GameObject bu = Instantiate(bubble, other.transform.position, Quaternion.identity);
                bu.transform.parent = other.transform;
                ThoughtBubble tb = bu.GetComponent<ThoughtBubble>();
                tb.SetText(other.GetComponent<EntityController>().thoughts);
            }
        }
    }

	void OnTriggerExit(Collider other) {
        if (other.isEnemy()) {
            for (int i = 0; i < other.transform.childCount - 1; i++) {
                if (other.transform.GetChild(i).CompareTag("Thought")) Destroy(other.transform.GetChild(i).gameObject);
            }
        }
	}


	void Update() {
        transform.position = player.transform.position;
    }
}