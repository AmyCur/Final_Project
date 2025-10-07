using MathsAndSome;
using UnityEngine;


public class Sensitivity {
    public static RefreshRate refreshRate;

    const bool shouldChange=true;

    [RuntimeInitializeOnLoadMethod]
    public static void Start() {

        if (shouldChange) {
			try {
				PL_Controller pc = mas.player.GetPlayer();
				if(pc!=null) if (refreshRate.value > 60) { pc.mouseSensitivityX /= 2f; pc.mouseSensitivityY /= 2f; }
			}
			catch {}
            
        }
        

    }
}
