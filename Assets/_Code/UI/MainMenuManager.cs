using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Cur.UI;

public class MainMenuManager : MonoBehaviour {
	// True when buttons are in the centre, false when they have been slid to the left

	public TMP_Text playText;
	public TMP_Text settingsText;
	public TMP_Text creditsText;
	public TMP_Text quitText;

	public Popup quitPopup;

	[System.Serializable]
	public class SlideableMenu {
		public bool activated = true;
		public Coroutine routine;
		public Image menu;
		public Vector2 t;
		[Header("Positions")]
		public Vector2 startPos;
		public Vector2 endPos;
		public float speed = 7;

		public IEnumerator SlideAnimation(Vector2 target, float speed = 5f) {
			this.t = new(this.menu.rectTransform.anchoredPosition.x, this.menu.rectTransform.anchoredPosition.y);

			while (Mathf.Abs(target.x - this.t.x) > 0.01f || Mathf.Abs(target.y - this.t.y) > 0.01f) {
				this.t.x = Mathf.Lerp(this.t.x, target.x, Time.deltaTime * speed);
				this.t.y = Mathf.Lerp(this.t.y, target.y, Time.deltaTime * speed);
				yield return 0;
			}

			this.t = target;

		}

		public void Slide(MonoBehaviour m) {
			if (this.routine != null) m.StopCoroutine(this.routine);
			this.routine = m.StartCoroutine(this.SlideAnimation(new Vector2(!activated ? endPos.x : startPos.x, !activated ? endPos.y : startPos.y), speed));
			this.activated = !this.activated;
		}
	}


	[SerializeField] SlideableMenu buttons;
	[SerializeField] SlideableMenu SettingsPage;
	[SerializeField] SlideableMenu CreditsPage;

	Coroutine slideRoutine;

	void Update() {
		buttons.menu.rectTransform.anchoredPosition = new Vector2(buttons.t.x, buttons.t.y);
		SettingsPage.menu.rectTransform.anchoredPosition = new Vector2(SettingsPage.t.x, SettingsPage.t.y);
		CreditsPage.menu.rectTransform.anchoredPosition = new Vector2(CreditsPage.t.x, CreditsPage.t.y);
	}

	void Start() {
		buttons.t = buttons.startPos;
		SettingsPage.t = SettingsPage.startPos;
		CreditsPage.t = CreditsPage.startPos;
	}

	public void SlideButtons() {
		if (buttons.routine != null) StopCoroutine(buttons.routine);
		buttons.routine = StartCoroutine(buttons.SlideAnimation(buttons.endPos, buttons.speed));
		buttons.activated = !buttons.activated;

		// playText.text = "▶";
		// settingsText.text = "";
		// creditsText.text = "";
		// quitText.text = "";
	}

	public void UnSlideButtons() {
		if (buttons.routine != null) StopCoroutine(buttons.routine);
		buttons.routine = StartCoroutine(buttons.SlideAnimation(buttons.startPos, buttons.speed));
		buttons.activated = !buttons.activated;

		// playText.text = "Play";
		// settingsText.text = "Settings";
		// creditsText.text = "Credits";
		// quitText.text = "Quit";
	}

	public void PlayGame() {
		SceneManager.LoadScene("Tutorial");
	}

	public void LoadCredits() {
		if (SettingsPage.activated) SettingsPage.Slide(this);
		else if (CreditsPage.activated) {
			UnSlideButtons();
		}
		else {
			SlideButtons();
		}

		CreditsPage.Slide(this);
	}



	public void LoadSettings() {
		if (CreditsPage.activated) CreditsPage.Slide(this);
		else if (SettingsPage.activated) {
			UnSlideButtons();
		}
		else {
			SlideButtons();
		}
		SettingsPage.Slide(this);
	}

	public void QuitGamePopup() {
		Create.CreatePopup(this, quitPopup);
	}

	public void QuitGame() => Application.Quit();

	
}
