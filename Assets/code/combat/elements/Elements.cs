using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Entity;

namespace Combat.Elements {
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
}