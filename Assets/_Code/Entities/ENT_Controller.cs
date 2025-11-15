using Elements;
using GameDebug;
using MathsAndSome;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity {
	// [RequireComponent(typeof(Rigidbody))]           // For movement
	public abstract class ENT_Controller : MonoBehaviour {

		public Dictionary<string, object> thoughts;


		[Header("Entity")]
		[Header("Stats")]
		public Health health;
		public float defence;

		public List<Element> currentElements;

		// [Header("Burning")]

		[HideInInspector] public float burnDamage = 10;
		[HideInInspector] public float burnDuration = 5f;
		[HideInInspector] public float currentBurnTime = 5f;
		[HideInInspector] public bool burning;





		protected float Positive(float value) {
			return Mathf.Clamp(value, 0, Mathf.Infinity);
		}

		// public virtual void OnValidate() { if (health <= 0) Debug.LogWarning($"Health Invalid on {gameObject.name}"); }

		#region Elemental Reaction Effects

		void ElectricFireDamage() { }
		void WaterFireDamage() { }
		void FireWindDamage() { }
		void WaterElectricDamage() { }
		void WaterWindDamage() { }





		public virtual void Die() { }

		public virtual void Update() {
			if (health <= 0) Die();
			UpdateThoughts();
		}
		// Should do JSON Loading of data here

		public virtual void Start() { }




		public virtual void UpdateThoughts() { }




		void ElectricWindDamage() {
			burnDamage = Consts.lightningNatureBurnDamage;
			burnDuration = Consts.lightningNatureBurnTime;

			if (burning) {
				currentBurnTime = burnDuration;
			}
			else {
				StartCoroutine(Burn());
			}
		}

		IEnumerator Burn() {
			burning = true;
			yield return new WaitForSeconds(1);
			TakeNonElementalDamage(burnDamage);
			currentBurnTime -= 1;

			if (currentBurnTime > 0) {
				StartCoroutine(Burn());
			}
			else {
				burning = false;
			}
		}

		#endregion


		void ApplyElementalDamage() {
			ElementType recent = currentElements[currentElements.Count - 1].type;
			ElementType other = currentElements[currentElements.Count - 2].type;

			burnDuration = Consts.burnTime;
			burnDamage = Consts.burnDamage;

			// I cant think of a better way :cry:

			#region Massive if block
			if (recent == ElementType.fire || other == ElementType.fire) {
				if (recent == ElementType.electric || other == ElementType.electric) {
					ElectricFireDamage();
				}
				if (recent == ElementType.water || other == ElementType.water) {
					WaterFireDamage();
				}
				if (recent == ElementType.wind || other == ElementType.wind) {
					FireWindDamage();
				}
			}

			if (recent == ElementType.water || other == ElementType.water) {
				if (recent == ElementType.electric || other == ElementType.electric) {
					WaterElectricDamage();
				}
				if (recent == ElementType.wind || other == ElementType.wind) {
					WaterWindDamage();
				}
			}

			if (recent == ElementType.electric || other == ElementType.electric) {
				if (recent == ElementType.wind || other == ElementType.wind) {
					ElectricWindDamage();
				}
			}
			#endregion
		}

		public void ApplyElements(Element element) {
			float newTime = Consts.randomDecayVariation(element.type);

			if (!currentElements.Contains(element)) {
				currentElements.Add(element);
				if (newTime >= element.f) {
					element.f = Consts.randomDecayVariation(element.type);
				}

				StartCoroutine(element.DecayElement(this));
			}
			else {
				if (newTime >= element.f) element.f = Consts.randomDecayVariation(element.type);
			}

			if (element.type == ElementType.electric) {

			}

			if (currentElements.Count > 1) {
				ApplyElementalDamage();
			}

		}

		#region Damage
		public void TakeDamage(float damage, Element element, float armourPenetration = 0) {
			float defenceDmgReduction = Positive(defence - armourPenetration) / 2;
			damage -= defenceDmgReduction;

			if (element.type == ElementType.electric) {
				ElementalFuncs.CreateLighningArc(element, gameObject);
			}

			ApplyElements(element);
			health -= Positive(damage);
		}

		public void TakeNonElementalDamage(float damage, float armourPenetration = 0) {
			float defenceDmgReduction = Positive(defence - armourPenetration) / 2;
			damage -= defenceDmgReduction;
			health -= Positive(damage);
		}
		#endregion

		void OnDrawGizmos() {
			Gizmos.color = Color.red;
			if (GameDebug.Combat.drawLightningArcRadius) Gizmos.DrawWireSphere(transform.position, Consts.lightningArcRange);
		}
	}
}