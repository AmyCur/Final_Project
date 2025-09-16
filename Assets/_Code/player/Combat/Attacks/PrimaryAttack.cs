using Magical;

public abstract class PrimaryAttack : SingularAttack {
    public override bool keyDown() => magic.key.down(keys.attack);
    public override bool keyStayDown() => magic.key.gk(keys.attack);
}