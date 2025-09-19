using Magical;
using UnityEngine;

public class PrimaryAttack : SingularAttack {
    public override bool keyDown() => magic.key.down(keys.attack);
    public override bool keyStayDown() => magic.key.gk(keys.attack);
    public override bool keyUp() => magic.key.up(keys.attack);

}