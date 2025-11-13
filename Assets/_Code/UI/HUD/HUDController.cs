using Elements;
using MathsAndSome;
using System.Collections.Generic;
using System.Collections;
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

	[System.Serializable]
	public class LerpableImage{
		public Image img;
		public Coroutine routine;
	}

	[Header("Images")]

	[SerializeField] Image weaponIcon;
	[SerializeField] LerpableImage[] weapons;

	[Header("Sliders")]

	[SerializeField] Slider[] staminaBars;
	[SerializeField] Slider healthBar;
	[SerializeField] Slider altCDBar;

	[Header("Text")]

	[SerializeField] TMP_Text healthText;

	[Header("Weapons")]

	[SerializeField] float currentWeaponScale = 1.2f;
	[SerializeField] float inactiveWeaponScale = 1f;
	[SerializeField] Color inactiveBackgroundColor = new Color(1f,0f,0f);
	[SerializeField] Color currentBackgroundColor = new Color(230f/255f, 209f/255f, 165f/255f);

	public void UpdateIcon(ElementType e) => weaponIcon.sprite = Resources.Load<Sprite>(ETypePath[e]);

	public void UpdateWeaponBackgrounds(){
		CombatController cc = pc.GetComponent<CombatController>();

		foreach(LerpableImage w in weapons){
			w.img.color = inactiveBackgroundColor;
		}

		weapons[cc.caIndex].img.color=currentBackgroundColor;
	}


	IEnumerator LerpScale(Image w, float targetScale){
		Vector3 s = w.transform.localScale;
		updatingWeapons=false;

		while(s != new Vector3(targetScale, targetScale, targetScale)){
			s = new Vector3(
					Mathf.Lerp(s.x, targetScale, Time.deltaTime * 30f),
					Mathf.Lerp(s.y, targetScale, Time.deltaTime * 30f),
					Mathf.Lerp(s.z, targetScale, Time.deltaTime * 30f)
			);
			w.transform.localScale=s;
			if(updatingWeapons) break;
			yield return 0;
		}
		updatingWeapons=false;
	}

	public void UpdateWeaponScale(){
		CombatController cc = pc.GetComponent<CombatController>();
		for(int i = 0; i < weapons.Length; i++){
			if(weapons[i].routine != null) StopCoroutine(weapons[i].routine);
			if(i==cc.caIndex) weapons[i].routine = StartCoroutine(LerpScale(weapons[i].img, currentWeaponScale));
			else weapons[i].routine = StartCoroutine(LerpScale(weapons[i].img, inactiveWeaponScale));
		}
	}

	bool updatingWeapons=false;
	public void UpdateWeapons(){
		updatingWeapons=true;
		UpdateWeaponBackgrounds();
		UpdateWeaponScale();
	}

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

	public void UpdateAltCD() {
		CombatController cc = pc.GetComponent<CombatController>();
		// Maxes at 1
		altCDBar.value = (float) cc.ca.alt.cooldownProgress / cc.ca.alt.attackCDIncrements * 100f;
		Debug.Log(cc.ca.alt.cooldownProgress / cc.ca.alt.attackCDIncrements);
	}

	public void UpdateAll(ElementType? IconType = null) {
		if (IconType != null) UpdateIcon((ElementType) IconType);
		UpdateStaminaBars();
	}

	void Awake() {
		pc = mas.player.GetPlayer();
	}




}