using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGJ14
{
    public class PlayerInputController : RobotInputController
    {
        public override Vector2 move => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        public override Vector2 camMove => new Vector2(Input.GetAxis("Mouse X") , Input.GetAxis("Mouse Y"));
        public override bool jump => Input.GetKey(KeyCode.Space);
        public override bool sprint => Input.GetKey(KeyCode.LeftShift);
        public override bool useLithiumBomb  => Input.GetKey(KeyCode.Q);
        public override bool dropUnitItens => Input.GetKey(KeyCode.R);
        public override bool dropHalfItens => Input.GetKey(KeyCode.LeftControl);
        public override bool shoot => Input.GetKey(KeyCode.Mouse0);
    }
}
