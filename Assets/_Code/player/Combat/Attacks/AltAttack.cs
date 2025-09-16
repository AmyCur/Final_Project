using Magical;

public abstract class AltAttack : SingularAttack {
    public override bool keyDown() => magic.key.down(keys.altAttack);
    public override bool keyStayDown() => magic.key.gk(keys.altAttack);
}