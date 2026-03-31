using Combat.Elements;
using Entities;
using FileManagement.Settings;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity {
	// [RequireComponent(typeof(Rigidbody))]           // For movement
	public abstract class ENT_Controller : MonoBehaviour {



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

		Coroutine burnRoutine;


		protected float Positive(float value) {
			return Mathf.Clamp(value, 0, Mathf.Infinity);
		}

		// public virtual void OnValidate() { if (health <= 0) Debug.LogWarning($"Health Invalid on {gameObject.name}"); }

		#region Elemental Reaction Effects

		void ElectricFireDamage() {
			gameObject.CreateDamageSphere(EnemyTypes.enemy, new Element(ElementType.None), 3f, 30f);
			StopCoroutine(burnRoutine);
		}
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
				if(burnRoutine!=null)StopCoroutine(burnRoutine);
				burnRoutine=StartCoroutine(Burn());
			}
		}

		IEnumerator Burn() {
			burning = true;
			yield return new WaitForSeconds(1);
			TakeNonElementalDamage(burnDamage);
			currentBurnTime -= 1;

			if (currentBurnTime > 0) {
				if(burnRoutine!=null)StopCoroutine(burnRoutine);
				burnRoutine=StartCoroutine(Burn());
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

		Coroutine damageRoutine;

		[SerializeField] protected Color baseColor;
		[SerializeField] protected Color damageTargetColor;
		[SerializeField] protected Color fireDamageTargetColor;

		public void HandleDamageAnimation(){
			if(damageRoutine!=null) StopCoroutine(damageRoutine);
			damageRoutine=StartCoroutine(DamageAnimation());
		}

		Color TargetColor(){

			foreach(Element e in currentElements){
				if(e.type==ElementType.fire){
					return fireDamageTargetColor;
					
				}
			}
			return damageTargetColor;
		}

		IEnumerator DamageAnimation(){
			MeshRenderer childMesh = transform.GetChild(0).GetComponent<MeshRenderer>();
			
			
			
			for(int i = 0; i<50; i++){


				Vector3 CV=
				mas.vector.LerpVectors(
					new Vector3(
						childMesh.material.color.r,
						childMesh.material.color.g,
						childMesh.material.color.b
					), 
					new Vector3(TargetColor().r,TargetColor().g,TargetColor().b),
					Time.deltaTime*17f
				);
				
				childMesh.material.color = new Color(CV.x,CV.y,CV.z);
				
				yield return 0;
			}

			for(int i = 0; i<200; i++){

				Vector3 CV=
				mas.vector.LerpVectors(
					new Vector3(
						childMesh.material.color.r,
						childMesh.material.color.g,
						childMesh.material.color.b
					), 
					new Vector3(baseColor.r,baseColor.g,baseColor.b),
					
					Time.deltaTime*19f
				);
				
				childMesh.material.color = new Color(CV.x,CV.y,CV.z);
				
				yield return 0;
			}


		}

		#region Damage
		public void TakeDamage(float damage, Element element, float armourPenetration = 0) {
			HandleDamageAnimation();
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
			if (FileManagement.Settings.Combat.drawLightningArcRadius) Gizmos.DrawWireSphere(transform.position, Consts.lightningArcRange);
		}
	}
}