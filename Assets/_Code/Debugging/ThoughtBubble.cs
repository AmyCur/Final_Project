using MathsAndSome;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThoughtBubble : MonoBehaviour {
    public TMP_Text variablesText;

    public string SetVariablesText(Dictionary<string, object> variables) {
        string kvps = "";
        if (variables != null) {
            foreach (KeyValuePair<string, object> kvp in variables) {
                kvps += $"{kvp.Key} : {kvp.Value} \n";
            }
        }
        return kvps;
    }

    public void SetText(Dictionary<string, object> vars) => variablesText.text = SetVariablesText(vars);

    Transform player => mas.player.Player.transform;

    void Awake() {
        variablesText = GetComponent<TMP_Text>();
        if (!variablesText) Debug.LogError($"Variables text is null on {gameObject.name}");
    }

    void Update() {
        transform.LookAt(player);
    }





}