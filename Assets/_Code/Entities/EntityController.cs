using UnityEngine;
using Elements;
using System.Collections.Generic;
using System.Collections;
using GameDebug;

public class EntityController : MonoBehaviour {
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

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (Combat.drawLightningArcRadius) Gizmos.DrawWireSphere(transform.position, Consts.lightningArcRange);
	}
}
