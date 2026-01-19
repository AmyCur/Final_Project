using UnityEngine;
using Entities;
using Elements;
using Magical;
using System.Collections;
using Combat.Enemies;

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
		
		Collider[] hits = Physics.OverlapBox(pc.playerCamera.transform.position+(pc.playerCamera.transform.forward*2f), new Vector3(3,3,3));
		
		foreach(Collider c in hits){
			ENM_Controller enm = c.GetComponent<ENM_Controller>();
			if (enm != null){
				enm.TakeDamage(damage, new(ElementType.None));
				pc.health+=lifeStealOnHit;
			}
		}
        base.OnClick();

		// if(Physics.Raycast(pc.playerCamera.transform.position, pc.playerCamera.transform.forward, out RaycastHit hit, range)){
		// 	ENM_Controller enm = hit.collider.GetComponent<ENM_Controller>();
		// 	if (enm != null){
		// 		enm.TakeDamage(damage, new(ElementType.None));
		// 		pc.health+=lifeStealOnHit;
		// 	}
		// }
    }

	public IEnumerator SwingCD(){
		yield return new WaitForSeconds(0.3f);
		if(!animator.GetBool("Attacking"))  animator.SetBool("Alternate", false);
	}

	
}
