using UnityEngine;
using System.Collections;

namespace Combat.Elements;

[System.Serializable]
public class Element {
	public ElementType type;
	public float f;

	public IEnumerator DecayElement(Entity.ENT_Controller ec) {
		yield return new WaitForSeconds(0.1f);
		f -= 0.1f;
		if (f > 0) {
			ec.StartCoroutine(DecayElement(ec));
		}
		else {
			ec.currentElements.Remove(this);
		}

	}

	public void StartDecay(Entity.ENT_Controller ec) {
		ec.StartCoroutine(DecayElement(ec));
	}

	public void RestartDecay(Entity.ENT_Controller ec) {
		StartDecay(ec);
	}

	public Element(ElementType type) {
		this.type = type;
	}
}
