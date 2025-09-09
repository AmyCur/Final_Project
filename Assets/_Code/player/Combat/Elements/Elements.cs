using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elements {
    public enum ElementType : short {
        None = 0,
        fire = 1,
        water = 2,
        electric = 4,
        nature = 8
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
            {ElementType.nature, 3f},
        };
    }

    [System.Serializable]
    public class Element {
        public ElementType type;
        public float f;

        public IEnumerator DecayElement(EntityController ec) {
            yield return new WaitForSeconds(0.1f);
            f -= 0.1f;
            if (f > 0) {
                ec.StartCoroutine(DecayElement(ec));
            }
            else {
                ec.currentElements.Remove(this);
            }

        }

        public void StartDecay(EntityController ec) {
            ec.StartCoroutine(DecayElement(ec));
        }

        public void RestartDecay(EntityController ec) {
            StartDecay(ec);

        }
    }

    public static class ElementalFuncs {
        public static void CreateLighningArc(Element element, GameObject obj) {
            List<Collider> cols = Physics.OverlapSphere(obj.transform.position, Consts.lightningArcRange).ToList();

            cols.Remove(obj.GetComponent<BoxCollider>());

            foreach (Collider col in cols) {
                EntityController ec = col.GetComponent<EntityController>();
                if (ec != null) {
                    ec.ApplyElements(element);
                }
            }
        }
    }
}