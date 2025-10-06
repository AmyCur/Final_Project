using Elements;
using MathsAndSome;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    // Paths
    PL_Controller pc;

    const string baseIconPath = "UI/Icons/Elements/";
    const string windPath = baseIconPath + "Wind";
    const string firePath = baseIconPath + "Fire";
    const string electricPath = baseIconPath + "Electric";
    const string waterPath = baseIconPath + "Water";

    readonly Dictionary<ElementType, string> ETypePath = new() {
        {ElementType.wind, windPath},
        {ElementType.fire, firePath},
        {ElementType.electric, electricPath},
        {ElementType.water, waterPath},
    };

    [Header("Images")]

    [SerializeField] Image weaponIcon;

    [Header("Sliders")]

    [SerializeField] Slider[] staminaBars;

    public void UpdateIcon(ElementType e) => weaponIcon.sprite = Resources.Load<Sprite>(ETypePath[e]);

    public void UpdateStaminaBars() {
        
        float stamina = pc.stamina;
        float maxStamina = pc.maxStamina;
        float minStamina = pc.minStamina;

        float maxStaminaPerBar = pc.maxStamina / staminaBars.Length;
        float currentStamina = stamina;


        foreach (Slider s in staminaBars) {
            s.value = Mathf.Clamp(currentStamina, minStamina, maxStaminaPerBar);
            currentStamina -= maxStaminaPerBar;
        }

        
    }

    public void UpdateAll(ElementType? IconType=null) {
        if (IconType != null) UpdateIcon((ElementType) IconType);
        UpdateStaminaBars();
    }

    void Awake() {
        pc = mas.player.GetPlayer();
    }
    
    
    
    
}
