using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

	// True when buttons are in the centre, false when they have been slid to the left
	bool buttonsCentral = true;

	[SerializeField] Image buttons;


	public void SlideButtons() {
		// Rect transform works from middle left
		if (buttonsCentral) buttons.rectTransform.anchoredPosition = new Vector2(-500f, buttons.rectTransform.anchoredPosition.y);
		else buttons.rectTransform.anchoredPosition = new Vector2(0, buttons.rectTransform.anchoredPosition.y);

		buttonsCentral = !buttonsCentral;

	}

	public void PlayGame() { }
	public void LoadCredits(){}
	public void LoadSettings(){}
}
