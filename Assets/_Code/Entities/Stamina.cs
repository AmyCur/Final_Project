namespace Entities;

[Serializable]
public class Stamina {
	public float min;
	public float max;
	public float s;
	public bool regenerating;
	public float regenTime=0.03f;
	public float staminaPerTick = 1f;

	[HideInInspector] public PL_Controller pc;

	public IEnumerator RegenerateStamina() {
		regenerating = true;
		while (s < max) {
			yield return new WaitForSeconds(regenTime / (pc.Grounded() ? 1.3f : 1f));
			// if (pc.state != PlayerState.sliding) s++;
		}

		if (s > max) s = max;
		regenerating = false;
	}



	public void Add(float value) {
		s += value;
		if (pc.hc != null) pc.hc.UpdateStaminaBars();
	}

	public void Subtract(float value) {
		s -= value;
		if (pc.hc != null) pc.hc.UpdateStaminaBars();
	}
}