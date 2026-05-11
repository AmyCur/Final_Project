using TMPro;
using UnityEngine;

public class FadeBasedOnDistance : MonoBehaviour
{
    void Update(){
        Vector3 screenPoint = Input.mousePosition;
		Vector3 pos = transform.position;
		float difference = 1-(Vector3.Magnitude(screenPoint-pos)/1000f);
		GetComponent<TMP_Text>().color = new Color(1,1,1,Mathf.Clamp(difference, 0.12f, 1f));
	}
}
