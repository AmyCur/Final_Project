namespace UI.HUD.Controller;

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