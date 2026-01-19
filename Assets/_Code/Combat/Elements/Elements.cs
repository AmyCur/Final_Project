using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Entity;

namespace Combat.Elements;

public enum ElementType : short {
	None = 0,
	fire = 1,
	water = 2,
	electric = 4,
	wind = 8
};

static class Consts {
	readonly public static float lightningArcRange = 2f;
	public static float randomDecayVariation(ElementType eType) => elementDecayDic[eType] * Random.Range(1, 1.4f);

	public static float burnDamage = 10f;
	public static float burnTime = 5f;
	public static float lightningNatureBurnDamage = 30f;
	public static float lightningNatureBurnTime = 4f;


	public static Dictionary<ElementType, float> elementDecayDic = new() {
		{ElementType.fire, 5f},
		{ElementType.water, 5f},
		{ElementType.electric, 2f},
		{ElementType.wind, 3f},
		{ElementType.None, 0f}
	};
}

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

public static class ElementalFuncs {
	public static void CreateLighningArc(Element element, GameObject obj) {
		List<Collider> cols = Physics.OverlapSphere(obj.transform.position, Consts.lightningArcRange).ToList();

		cols.Remove(obj.GetComponent<BoxCollider>());

		foreach (Collider col in cols) {
			Entity.ENT_Controller ec = col.GetComponent<Entity.ENT_Controller>();
			if (!!ec) {
				ec.ApplyElements(element);
			}
		}
	}
}
