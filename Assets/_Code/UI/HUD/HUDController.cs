using Elements;
using MathsAndSome;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cur.UI {
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
		public class LerpableImage {
			public Image img;
			public Coroutine routine;
			bool updatingWeapons;

			public IEnumerator LerpScale(float targetScale) {
				Vector3 s = img.transform.localScale;
				updatingWeapons = false;

				while (s != new Vector3(targetScale, targetScale, targetScale)) {
					s = new Vector3(
						Mathf.Lerp(s.x, targetScale, Time.deltaTime * 30f),
						Mathf.Lerp(s.y, targetScale, Time.deltaTime * 30f),
						Mathf.Lerp(s.z, targetScale, Time.deltaTime * 30f)
					);
					img.transform.localScale = s;
					if (updatingWeapons) break;
					yield return 0;
				}
				updatingWeapons = false;
			}

			public void RotateWeapon() {
				Animator weapon = img.transform.GetChild(0).GetComponent<Animator>();
				weapon.SetBool("WeaponSelected", true);
			}

		}

		[Header("Images")]

		[SerializeField] Image weaponIcon;
		[SerializeField] LerpableImage[] weapons;

		Image altBarBG => altBar.transform.GetChild(0).GetComponent<Image>();
		Image altBarFill => altBar.fillRect.GetComponent<Image>();

		[Header("Sliders")]

		[SerializeField] Slider[] staminaBars;
		[SerializeField] Slider healthBar;
		[SerializeField] Slider altBar;

		[Header("Text")]

		[SerializeField] TMP_Text healthText;

		[Header("Weapons")]

		[SerializeField] float currentWeaponScale = 1.2f;
		[SerializeField] float inactiveWeaponScale = 1f;
		[SerializeField] Color inactiveBackgroundColor = new Color(1f, 0f, 0f);
		[SerializeField] Color currentBackgroundColor = new Color(230f / 255f, 209f / 255f, 165f / 255f);

		Combat.CombatController cc;

		[Header("Colors")]

		[SerializeField] Color fireColor;
		[SerializeField] Color waterColor;
		[SerializeField] Color electricColor;
		[SerializeField] Color windColor;

		[Header("Floats")]

		[SerializeField] float altCDBarColorSpeed = 20f;

		Dictionary<ElementType, Color> ElementColor => new(){
			{ElementType.electric, electricColor},
			{ElementType.fire, fireColor},
			{ElementType.water, waterColor},
			{ElementType.wind, windColor}

		};


		public void UpdateIcon(ElementType e) => weaponIcon.sprite = Resources.Load<Sprite>(ETypePath[e]);

		public void UpdateWeaponBackgrounds() {

			foreach (LerpableImage w in weapons) {
				w.img.color = inactiveBackgroundColor;
			}

			weapons[cc.caIndex].img.color = currentBackgroundColor;
		}

		public void UpdateWeaponScale() {
			for (int i = 0; i < weapons.Length; i++) {
				if (weapons[i].routine != null) StopCoroutine(weapons[i].routine);
				if (i == cc.caIndex) weapons[i].routine = StartCoroutine(weapons[i].LerpScale(currentWeaponScale));
				else weapons[i].routine = StartCoroutine(weapons[i].LerpScale(inactiveWeaponScale));
			}
		}

		public void RotateWeapons() {
			for (int i = 0; i < weapons.Length; i++) {
				if (i == cc.caIndex) weapons[i].RotateWeapon();
			}
		}

		public void UpdateWeapons() {
			UpdateWeaponBackgrounds();
			UpdateWeaponScale();
			RotateWeapons();
		}

		public IEnumerator UpdateAltCDBarColor(ElementType element, bool lerp = true) {
			Color targetColor = ElementColor[element];

			if (!lerp) {
				altBarBG.color = targetColor.Darken();
				altBarFill.color = targetColor;
				yield return 0;
			}
			else {
				Color darkColor = targetColor.Darken();
				while (altBarFill.color != targetColor) {
					float t = Time.deltaTime * altCDBarColorSpeed;
					altBarFill.color = new Color(
						Mathf.Lerp(altBarFill.color.r, targetColor.r, t),
						Mathf.Lerp(altBarFill.color.g, targetColor.g, t),
						Mathf.Lerp(altBarFill.color.b, targetColor.b, t)
					);

					altBarBG.color = new Color(
					Mathf.Lerp(altBarBG.color.r, darkColor.r, t),
					Mathf.Lerp(altBarBG.color.g, darkColor.g, t),
					Mathf.Lerp(altBarBG.color.b, darkColor.b, t)
					);

					yield return 0;
				}
			}
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
			healthBar.value = pc.health.h;
			healthText.text = Mathf.CeilToInt(pc.health.h).ToString() + " +";
		}

		public void UpdateAltCD() {
			// Maxes at 1
			altBar.value = (float) cc.ca.alt.cooldownProgress / cc.ca.alt.attackCDIncrements * 100f;

			Debug.Log(cc.ca.alt.cooldownProgress / cc.ca.alt.attackCDIncrements);
		}

		public void UpdateAll(ElementType? IconType = null) {
			if (IconType != null) UpdateIcon((ElementType) IconType);
			UpdateStaminaBars();
			UpdateWeapons();
		}

		public void UpdateWeaponIcons() {
			for (int i = 0; i < weapons.Length; i++) {
				//FIXME: There has to be a better way to do this
				try {
					Combat.SingleAttack atk = cc.attacks[i];
					weapons[i].img.gameObject.SetActive(true);
				}
				catch {
					weapons[i].img.gameObject.SetActive(false);
				}
			}
		}


		void Awake() {
			pc = mas.player.GetPlayer();
			cc = pc.GetComponent<Combat.CombatController>();

			UpdateWeaponIcons();
		}

		void Update() {
			Debug.Log(altBar.fillRect.GetComponent<Image>().color);

		}



	}
}