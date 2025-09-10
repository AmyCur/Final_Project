using MyGameUILibrary;
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
                left = 20, top = 20, width = 200, height = 200
            }
        };

        root.Add(m_RadialProgress);
    }

    void Update() {
        // m_RadialProgress.progress = ((Mathf.Sin(Time.time) + 1.0f) / 2.0f) * 60.0f + 10.0f;
    }

    public void UpdateProgress(float v) {
        m_RadialProgress.progress = v;
        
    }
}