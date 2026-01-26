using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Combat.Elements;

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