using Magical;
using UnityEngine;


public class AlternateAttack : SingularAttack {
    public override bool keyDown() => magic.key.down(keys.altAttack);
    public override bool keyStayDown() => magic.key.gk(keys.altAttack);
    public override bool keyUp() => magic.key.up(keys.altAttack);
}