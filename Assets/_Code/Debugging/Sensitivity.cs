using MathsAndSome;
using UnityEngine;

public class Sensitivity {
    public static RefreshRate refreshRate;

    const bool shouldChange=true;

    [RuntimeInitializeOnLoadMethod]
    public static void Start() {

        if (shouldChange) {
            try
            {
                Player.Movement.PL_Controller pc = mas.player.Player;
                if (pc != null) if (refreshRate.value > 60) { pc.mouseSensitivityX /= 2f; pc.mouseSensitivityY /= 2f; }
            }
            catch
            {
                Debug.Log("anti fun loser");
            }
            
        }
        

    }
}
