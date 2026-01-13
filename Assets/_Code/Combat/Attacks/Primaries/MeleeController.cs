using UnityEngine;
using EntityLib;
using Elements;

[CreateAssetMenu(fileName="Melee Attack", menuName="Attacks/Create/Melee")]
public class MeleeAttack : PrimaryAttack{

		
	public Animator animator => GameObject.Find("PlayerWeapon").GetComponent<Animator>();

	void RotateModel(){
		animator.SetBool("Attacking", true);
	}

    public override void OnClick(){
		RotateModel();
		if(Physics.Raycast(pc.playerCamera.transform.position, pc.playerCamera.transform.forward, out RaycastHit hit, range)){
			ENM_Controller enm = hit.collider.GetComponent<ENM_Controller>();
			if (enm != null){
				enm.TakeDamage(damage, new(ElementType.None));
			}
		}
        base.OnClick();
    }
}