using Elements;
using MathsAndSome;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour {
	// Paths
	Player.PL_Controller pc;

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
	[SerializeField] Slider healthBar;

	[Header("Text")]

	[SerializeField] TMP_Text healthText;

	public void UpdateIcon(ElementType e) => weaponIcon.sprite = Resources.Load<Sprite>(ETypePath[e]);

	public void UpdateStaminaBars() {

		float stamina = pc.stamina.s;
		float maxStamina = pc.stamina.max;
		float minStamina = pc.stamina.min;

		float maxStaminaPerBar = pc.stamina.max / staminaBars.Length;
		float currentStamina = stamina;


		foreach (Slider s in staminaBars) {
			s.value = Mathf.Clamp(currentStamina, minStamina, maxStaminaPerBar);
			currentStamina -= maxStaminaPerBar;
		}
	}

	public void UpdateHeath() {
		healthBar.value = pc.health.health;
		healthText.text = Mathf.CeilToInt(pc.health.health).ToString() + " +";
	}

	public void UpdateAll(ElementType? IconType = null) {
		if (IconType != null) UpdateIcon((ElementType) IconType);
		UpdateStaminaBars();
	}

	void Awake() {
		pc = mas.player.GetPlayer();
	}




}