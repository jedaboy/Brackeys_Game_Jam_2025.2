using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGJ14
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class CharacterController : MonoBehaviour
    {

      public Animator anim;
      public new Rigidbody rigidbody;
      public CapsuleCollider capsuleCollider;
      public Battery battery;

    }
}