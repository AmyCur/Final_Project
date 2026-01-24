using UnityEngine;
using MathsAndSome;
using Player.Movement;

namespace Player {
    public static class Directions{
        public static Vector3 SlideDirection(this PL_Controller pc, bool increaseForward = false) => pc.DashDirection(increaseForward);
            
        public static Vector3 DashDirection(this PL_Controller pc, bool increaseForward = true)
        {
            float hInp = Input.GetAxisRaw("Horizontal");
            float vInp = Input.GetAxisRaw("Vertical");

            Vector3 forward = pc.forwardObject.transform.forward;
            Vector3 right = pc.forwardObject.transform.right;


            if (hInp != 0) return vInp != 0 ? (forward * vInp + right * hInp).normalized : (right * hInp).normalized;
            else return vInp != 0 ? (forward * vInp).normalized : increaseForward ? forward * 1.1f : forward /*Forward is increased because it feels smaller cos ur not moving*/;
        }

		public static Vector3 forward => mas.player.Player.playerCamera.transform.forward;
		public static Vector3 backward => -mas.player.Player.playerCamera.transform.forward;
    }
    
}