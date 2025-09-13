using UnityEngine;
using Elements;
using System.Collections.Generic;
using System.Collections;
using GameDebug;
using MathsAndSome;

public abstract class EntityController : MonoBehaviour {
    [Header("Entity")]
    [Header("Stats")]
    public float health;
    public float defence;

    [Header("Elements")]
    public List<Element> currentElements;

    [Header("Burning")]

    public float burnDamage = 10;
    public float burnDuration = 5f;
    public float currentBurnTime = 5f;
    [HideInInspector] public bool burning;
    [HideInInspector] public Rigidbody rb;

    [Header("Movement")]
	public bool canMove = true;
    public Vector3 movementVector;


    [Header("Impulses")]
	public List<Impulse> SV; 

	[System.Serializable]
	public class Impulse {
		public Vector3 force;
        readonly EntityController ec;

		IEnumerator ReduceForce() {

			float dx = force.x / 2f;
			float dy = force.y / 2f;
			float dz = force.z / 2f;

			float ax = Mathf.Abs(force.x);
			float ay = Mathf.Abs(force.y);
			float az = Mathf.Abs(force.z);

            Vector3 sf = force;
			while (Mathf.Abs(force.x)+Mathf.Abs(force.y)+Mathf.Abs(force.z)>1) {
				force = new(force.x - dx, force.y - dy, force.z - dz);

                force = new(
                    Mathf.Clamp(force.x, sf.x<0 ? -10_000 : 0, sf.x>=0 ? 10_000 : 0),
                    Mathf.Clamp(force.y, sf.y<0 ? -10_000 : 0, sf.y>=0 ? 10_000 : 0),
                    Mathf.Clamp(force.z, sf.x<0 ? -10_000 : 0, sf.z>=0 ? 10_000 : 0)
                );
				
				
				Debug.Log(force);
				yield return new WaitForSeconds(.1f);
			}
			Debug.Log("fin");
			ec.SV.Remove(this);

		}

		public Impulse(Vector3 force, EntityController ec) {
			Debug.Log("Impulse Made");
            this.ec = ec;
			this.force = force;
			mas.player.GetPlayer().StartCoroutine(ReduceForce());
		}
	}


    bool areType(ElementType recent, ElementType other, ElementType type) {
        return recent == type || other == type;
    }

    protected float Positive(float value) {
        return Mathf.Clamp(value, 0, Mathf.Infinity);
    }

    #region Elemental Reaction Effects

    void ElectricFireDamage() { }
    void WaterFireDamage() { }
    void FireNatureDamage() { }
    void WaterElectricDamage() { }
    void WaterNatureDamage() { }

    public void Move() {
		// I should have the player be moved by adding all the movement vectors
		Vector3 impulses() {
			Vector3 t = new();
			foreach (Impulse i in SV) {
				t += i.force;
			}
			return t;
		}

		Vector3 velocity = movementVector + impulses();

		if (velocity.y == 0) {
			velocity = new(velocity.x, rb.linearVelocity.y, velocity.z);
		}

        rb.linearVelocity = mas.vector.ClampVectorWithFloat(velocity, -1000, 1000);
	}

    public virtual void SetStartDefaults() {
		rb = GetComponent<Rigidbody>();
    }

    public void Start() {
        SetStartDefaults();
    }

    public virtual void FixedUpdate() {
        if (canMove) {
            Move();
        }		
	}


    void ElectricNatureDamage() {
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
            if (recent == ElementType.nature || other == ElementType.nature) {
                FireNatureDamage();
            }
        }

        if (recent == ElementType.water || other == ElementType.water) {
            if (recent == ElementType.electric || other == ElementType.electric) {
                WaterElectricDamage();
            }
            if (recent == ElementType.nature || other == ElementType.nature) {
                WaterNatureDamage();
            }
        }

        if (recent == ElementType.electric || other == ElementType.electric) {
            if (recent == ElementType.nature || other == ElementType.nature) {
                ElectricNatureDamage();
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
        if (Combat.drawLightningArcRadius) Gizmos.DrawWireSphere(transform.position, Consts.lightningArcRange);
	}
}