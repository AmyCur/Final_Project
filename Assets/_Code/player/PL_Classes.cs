using System;
using System.Collections;
using UnityEngine;

namespace Player {

    [Serializable]
    public class Stamina {
        public float min;
        public float max;
        public float s;
        public bool regenerating;
        public float regenTime;
        [HideInInspector] public PL_Controller pc;

        public IEnumerator RegenerateStamina() {
            regenerating = true;
            while (s < max) {
                yield return new WaitForSeconds(regenTime / (pc.Grounded() ? 1.3f : 1f));
                // if (pc.state != PlayerState.sliding) s++;
            }

            if (s > max) s = max;
            regenerating = false;
        }

    }




    [Serializable]
    public class Force {

        public enum ForceState {
            start,
            middle,
            end
        }

        public ForceState   forceState;
        public float        force;
        public bool         can = true;
        public float        directionChangeSpeed;
        public Vector3      direction;
        public bool[]       goneBack = new bool[2];

        // public void DirectionChange() {
        //     Vector3 moveD = new(Math.Sign(PL_Controller.moveDirection.x), 0, Math.Sign(PL_Controller.moveDirection.z));
        //     Vector3 forceD = new(Math.Sign(this.direction.x), 0, Math.Sign(this.direction.z));

        //     // Check if the player is moving in the opposite direciton of the dash and if they are, change the dash direction to suit them

        //     if (-moveD.x == forceD.x && moveD.x != 0 && forceD.x != 0) {
        //         if (this.direction.x <= 0) this.direction = new(this.direction.x + this.directionChangeSpeed, this.direction.y, this.direction.z);
        //         else if (this.direction.x >= 0) this.direction = new(this.direction.x - this.directionChangeSpeed, this.direction.y, this.direction.z);
        //         goneBack[0] = true;
        //     }

        //     if (-moveD.z == forceD.z && moveD.z != 0 && forceD.z != 0) {
        //         if (this.direction.z <= 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z + this.directionChangeSpeed);
        //         else if (this.direction.z >= 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z - this.directionChangeSpeed);
        //         goneBack[1] = true;
        //     }

        //     // if (goneBack[0]) this.direction = new(Mathf.Clamp(this.direction.x, -.8f, .8f), this.direction.y, this.direction.z);
        //     // if (goneBack[1]) this.direction = new(this.direction.x, this.direction.y, Mathf.Clamp(this.direction.z, -.8f, .8f));
        // }

        public void ResetGoneBack() => this.goneBack = new bool[] { false, false };

        public void ResetDirection() => direction = Vector3.zero;
    }
}