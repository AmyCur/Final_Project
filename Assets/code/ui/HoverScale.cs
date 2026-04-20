using UnityEngine;
using MathsAndSome;
using System.Collections;

namespace UI{
    public class HoverScale : MonoBehaviour{
        public float targetScale=1.1f;

        Coroutine routine;

        IEnumerator LerpRoutine(float targetScale){
            while(transform.localScale!=new Vector3(targetScale, targetScale, targetScale)){
                transform.localScale=mas.vector.LerpVectors(transform.localScale, new Vector3(targetScale,targetScale,targetScale), Time.deltaTime);
                yield return 0;
            }
        }
        void OnMouseOver(){
            Debug.Log($"hover {gameObject.name}");
            if(routine!=null) StopCoroutine(routine);
            routine=StartCoroutine(LerpRoutine(targetScale));
            // transform.localScale=Mathf.Lerp(transform.localScale, new Vector3(targetScale,targetScale,targetScale), Time.deltaTime);
        }
        void OnMouseExit()
        {
            Debug.Log($"nhover {gameObject.name}");
            if(routine!=null) StopCoroutine(routine);
            routine=StartCoroutine(LerpRoutine(1));
            
        }

    }
}