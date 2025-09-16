
using Elements;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateWeaponIcon : MonoBehaviour {
    Image img;
    const string baseIconPath = "UI/Icons/Elements/";
    const string windPath = baseIconPath+"Wind";
    const string firePath = baseIconPath+"Fire";
    const string electricPath = baseIconPath+"Electric";
    const string waterPath = baseIconPath+"Water";

    readonly Dictionary<ElementType, string> ETypePath = new() {
        {ElementType.wind, windPath},
        {ElementType.fire, firePath},
        {ElementType.electric, electricPath},
        {ElementType.water, waterPath},
    };

    public void UpdateIcon(ElementType e) {
        img.sprite = Resources.Load<Sprite>(ETypePath[e]);
    }

    void Start() {
        img = GetComponent<Image>();

        if (img == null) {
            Debug.LogError($"The image on {gameObject.name} is null");
        }
	}
}
