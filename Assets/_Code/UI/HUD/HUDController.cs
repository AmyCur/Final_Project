using Elements;
using MathsAndSome;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cur.UI {
	public static class BarColors {
		public static Color fireColor = new Color(0.780f, 0.19f, 0.27f, 1);
		public static Color waterColor = new Color(0, 0, 1, 1);
		public static Color electricColor = new Color(1, 1, 0, 1);
		public static Color windColor = new Color(0, 1, 0, 1);
		public static Color noneColor = new Color(1, 1, 1, 1);

		public static Dictionary<ElementType, Color> ElementColor = new(){
			{ElementType.electric, electricColor},
			{ElementType.fire, fireColor},
			{ElementType.water, waterColor},
			{ElementType.wind, windColor},
			{ElementType.None, noneColor}
		};
	}

	[System.Serializable]
	public class CooldownBar {
		public Image parentBar;
		public Coroutine routine;
		public float colorSpeed = 20f;

		[HideInInspector] public Image background => parentBar.transform.GetChild(0).GetComponent<Image>();
		[HideInInspector] public Image bar => parentBar.transform.GetChild(1).GetComponent<Image>();
		[HideInInspector] public Image key => parentBar.transform.GetChild(2).GetComponent<Image>();
		[HideInInspector] public Image icon => parentBar.transform.GetChild(3).GetComponent<Image>();

		public IEnumerator UpdateBarColor(ElementType element, bool lerp = true) {

			Color targetColor = BarColors.ElementColor[element];
			Color darkColor = targetColor.Darken();
			Color lightColor = targetColor.Lighten(0.78f);

			if (!lerp) {
				// background.color = targetColor.Darken();
				bar.color = targetColor;
				icon.color = lightColor;

				yield return 0;
			}
			else {

				while (bar.color != targetColor) {
					float t = Time.deltaTime * colorSpeed;
					bar.color = new Color(
						Mathf.Lerp(bar.color.r, targetColor.r, t),
						Mathf.Lerp(bar.color.g, targetColor.g, t),
						Mathf.Lerp(bar.color.b, targetColor.b, t)
					);

					icon.color = new Color(
					Mathf.Lerp(icon.color.r, lightColor.r, t),
					Mathf.Lerp(icon.color.g, lightColor.g, t),
					Mathf.Lerp(icon.color.b, lightColor.b, t)
					);



					// background.color = new Color(
					// Mathf.Lerp(background.color.r, darkColor.r, t),
					// Mathf.Lerp(background.color.g, darkColor.g, t),
					// Mathf.Lerp(background.color.b, darkColor.b, t)
					// );

					yield return 0;
				}
			}
		}

		public void UpdateAbilityIcon(Sprite i) => icon.sprite = i;

		public void UpdateBarCD(float cooldownProgress, float cdIncrements) {
			// Maxes at 1
			bar.fillAmount = cooldownProgress / cdIncrements;
			// Debug.Log(cooldownProgress / cdIncrements * 100f);

		}

	}

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



		public CooldownBar assistBar;
		public CooldownBar abilityBar;

		[HideInInspector] public CooldownBar[] cdBars;

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

		// Image altBarBG => altBar.transform.GetChild(0).GetComponent<Image>();
		// Image fill => altBar.fillRect.GetComponent<Image>();

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


		[SerializeField] TMP_Text dash;


		//
		// [Header("Floats")]
		//
		// [SerializeField] float altCDBarColorSpeed = 20f;


		public void UpdateDash(string text){
			dash.text=text;
		}

		// public void UpdateIcon(ElementType e) => weaponIcon.sprite = Resources.Load<Sprite>(ETypePath[e]);

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


			healthBar.fillRect.GetComponent<Image>().color = ColorUtil.Lerp(Color.red, Color.green, pc.health.h / 100f);

			healthBar.transform.GetChild(0).GetComponent<Image>().color = ColorUtil.Lerp(Color.red.Darken(), Color.green.Darken(), pc.health.h / 100f);

			healthText.text = Mathf.CeilToInt(pc.health.h).ToString() + " +";

		}



		public void UpdateAll(ElementType? IconType = null) {
			// if (IconType != null) UpdateIcon((ElementType) IconType);
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


		public void WeaponSwitch() {
			// UpdateAbilityIcons();
			UpdateWeapons();
		}


		void Awake() {
			pc = mas.player.Player;
			cc = pc.GetComponent<Combat.CombatController>();
			cdBars = new CooldownBar[2] { assistBar, abilityBar };


			UpdateWeaponIcons();
		}

		void UpdateAllBarColors() {
			foreach (CooldownBar bar in cdBars) {
				if(bar!=null) StartCoroutine(bar.UpdateBarColor(cc.ca.primary.element.type, false));
			}
		}

		void Start() => UpdateAllBarColors();

	}
}