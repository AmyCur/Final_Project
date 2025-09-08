using System.Collections;
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
        public static float emDecayTime = 5f;    
    }

    [System.Serializable]
    public class Element {
        public ElementType type;
        public IEnumerator delay;
        public bool shouldRestart;

        IEnumerator DecayElement(EnemyController ec) {
            yield return new WaitForSeconds(Consts.emDecayTime);
            if (shouldRestart) {
                shouldRestart = false;
                ec.StartCoroutine(DecayElement(ec));
            }
            else {
                ec.currentElements.Remove(this);
                Debug.LogError($"Ending decay! | Decay >> {Consts.emDecayTime}");
            }
            
        
            
        }

        public void StartDecay(EnemyController ec) {
            ec.StartCoroutine(DecayElement(ec));
        }

        public void RestartDecay(EnemyController ec) {
            StartDecay(ec);
            
        }
    }
}