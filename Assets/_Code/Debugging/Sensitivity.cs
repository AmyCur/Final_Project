#define ADJUST_SENS
using MathsAndSome;
using UnityEngine;


public class Sensitivity {
    public static RefreshRate refreshRate;

    [RuntimeInitializeOnLoadMethod]
    public static void Start() {

#if ADJUST_SENS
        PlayerController pc = mas.player.GetPlayer();
        if (refreshRate.value > 60){ pc.mouseSensitivityX/=2f; pc.mouseSensitivityY/=2f;}
#endif
    }
}
