using UnityEngine;

public class WindAttack : Attack {
    public override void OnClick() {
        base.OnClick();
    }

    void CreateVortex() {
        
    }

    public override void OnALtClick() {
        base.OnALtClick();

        CreateVortex();
    }    
}
