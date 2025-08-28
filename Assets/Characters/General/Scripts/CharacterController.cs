using GRD.FSM;
using UnityEngine;

namespace BGJ14
{
    
    public class CharacterController : MonoBehaviour
    {

      public Animator anim;
      public new Rigidbody rigidbody;
      public CapsuleCollider capsuleCollider;
      public SphereCollider sphereCollider;
      public Battery battery;
      public FSM_Manager fsmManager;

    }
}