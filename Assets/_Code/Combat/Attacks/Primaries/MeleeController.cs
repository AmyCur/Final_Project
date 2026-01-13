using UnityEngine;
using EntityLib;
using Elements;
using Magical;
using System.Collections;

[CreateAssetMenu(fileName="Melee Attack", menuName="Attacks/Create/Melee")]
public class MeleeAttack : SingularAttack{

	public override bool keyDown() => magic.key.down(keys.attack);
    public override bool keyStayDown() => magic.key.gk(keys.attack);
    public override bool keyUp() => magic.key.up(keys.attack);

	public Animator animator => GameObject.Find("PlayerWeapon").GetComponent<Animator>();

	bool left=false;

	void RotateModel(){
		animator.SetBool("Attacking", true);
		animator.SetBool("Alternate", left);
		left=!left;
	}

    public override void OnClick(){
		RotateModel();
		Debug.Log("bLUA");
		if(Physics.Raycast(pc.playerCamera.transform.position, pc.playerCamera.transform.forward, out RaycastHit hit, range)){
			ENM_Controller enm = hit.collider.GetComponent<ENM_Controller>();
			if (enm != null){
				enm.TakeDamage(damage, new(ElementType.None));
			}
		}
        base.OnClick();
    }

	public IEnumerator SwingCD(){
		yield return new WaitForSeconds(0.3f);
		if(!animator.GetBool("Attacking")) animator.SetBool("Alternate", false);
	}
}
