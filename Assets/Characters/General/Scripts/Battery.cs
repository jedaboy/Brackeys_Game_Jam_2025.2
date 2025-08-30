using GRD.FSM;
using System;
using UnityEngine;

namespace  BGJ14
{


    public class Battery : MonoBehaviour
    {
        [SerializeField, Range(0f, 100f)]
        public float maxCharge = 100f;
        private FSM_Manager fsm_Manager;
        private CharacterController characterController;
        [SerializeField]
        public float currentCharge;
        public Action onBatteryUpdate;


        public float CurrentCharge => currentCharge;
    	public float NormalizedCurrentCharge => currentCharge / maxCharge;

        [SerializeField, Range(0f, 10f)]
        public float drainRate = 0.1f; // quanto drena por segundo

        public bool IsEmpty => currentCharge <= 0;


        void Awake()
        {
            currentCharge = maxCharge;
            characterController = this.GetComponent<CharacterController>();
            fsm_Manager = characterController.fsmManager;
        }

        public void Drain(float amount)
        {
            currentCharge -= amount;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
            onBatteryUpdate?.Invoke();
            if (IsEmpty)
            {
                fsm_Manager.SetBool("IsDead", true);
            }
        }

        public void Recharge(float amount)
        {

            currentCharge += amount;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
            onBatteryUpdate?.Invoke();

        }

        public void DrainOverTime()
        {
            if (!IsEmpty)
            {
                currentCharge -= drainRate * Time.deltaTime;
                currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
                onBatteryUpdate?.Invoke();

                if (IsEmpty)
                {
                    fsm_Manager.SetBool("IsDead", true);
                }
            }
        }

        public void FullRecharge()
        {
            currentCharge = maxCharge;
        }
    }
}