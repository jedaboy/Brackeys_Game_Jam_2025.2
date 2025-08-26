using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BGJ14
{
    public abstract class RobotInputController : MonoBehaviour
    {
        public abstract Vector2 move { get; }
        public abstract Vector2 camMove { get; }

        public abstract bool jump { get; }
        public abstract bool sprint { get; }

        public abstract bool dropItens { get; }

        public abstract bool shoot { get; }
        //public abstract bool sprint { get; }


    }
}