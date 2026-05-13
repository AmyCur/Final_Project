using UnityEngine;
using MathsAndSome;
using System.Collections;
using UnityEngine.EventSystems; 

namespace UI{
    public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
        public static float targetScale=1.1f;
        public static float lerpSpeed=10f;

        Coroutine routine;

        IEnumerator LarpRoutine(float targetScale){
            while(transform.localScale!=new Vector3(targetScale, targetScale, targetScale)){
                transform.localScale=mas.vector.LerpVectors(transform.localScale, new Vector3(targetScale,targetScale,targetScale), Time.deltaTime*lerpSpeed);
                yield return 0;
            }
        }
        public void OnPointerEnter(PointerEventData ped){
            Debug.Log($"hover {gameObject.name}");
            if(routine!=null) StopCoroutine(routine);
            routine=StartCoroutine(LarpRoutine(targetScale));
            // transform.localScale=Mathf.Lerp(transform.localScale, new Vector3(targetScale,targetScale,targetScale), Time.deltaTime);
        }
        public void OnPointerExit(PointerEventData ped)
        {
            Debug.Log($"nhover {gameObject.name}");
            if(routine!=null) StopCoroutine(routine);
            routine=StartCoroutine(LarpRoutine(1));
            
        }

    }
}