using RadialProgressLib;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class RadialUpdater : MonoBehaviour {
    RadialProgress m_RadialProgress;

    void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;

        m_RadialProgress = new RadialProgress() {
            style = {
                position = Position.Absolute,
                right = 20, bottom = 20, width = 120, height = 120,
                backgroundColor=Color.black,
            }
        };

        root.Add(m_RadialProgress);
    }

    void Update() {
        // m_RadialProgress.progress = ((Mathf.Sin(Time.time) + 1.0f) / 2.0f) * 60.0f + 10.0f;
    }

    public void UpdateProgress(float v) {
        m_RadialProgress.progress = Mathf.Clamp(v, 0, 100);
        
    }
}