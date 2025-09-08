using Elements;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    [SerializeField] protected float health;
    [SerializeField] protected float defence;

    public List<Element> currentElements;

    protected float Positive(float value) {
        return Mathf.Clamp(value, 0, Mathf.Infinity);
    }


    public void TakeDamage(float damage, Element element, float armourPenetration = 0) {
        float defenceDmgReduction = Positive(defence - armourPenetration) / 2;

        if (!currentElements.Contains(element)) {
            currentElements.Add(element);
            Debug.Log("DOESNT HAVE ELEMENT!");
            element.shouldRestart = false;
            element.StartDecay(this);
        }
        else {
            element.shouldRestart = true;
            element.RestartDecay(this);
            Debug.Log("HAS ELEMENT!");
        }
        
        
        // List<ElementType> types = new();

        // foreach (Element e in currentElements) {
        //     // If its already effected
        //     if (e.type == element.type) {
        //         // This will also skip over the element and not add it
        //         Debug.Log("SO TRUE");
        //         e.RestartDecay(this);
        //     }
        //     else {
        //         Debug.Log("SO FALSE");
        //         types.Add(e.type);

        //         e.StartDecay(this);
        //     }
        // }





        damage -= defenceDmgReduction;
        health -= Positive(damage);
        // Debug.LogWarning($"Health {health}");
    }
}