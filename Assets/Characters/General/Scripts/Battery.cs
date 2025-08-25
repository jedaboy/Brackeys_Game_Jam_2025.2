using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    private float maxCharge = 100f;   

    [SerializeField]
    private float currentCharge;      

    public float CurrentCharge => currentCharge;

    [SerializeField, Range(0f, 10f)]
    private float drainRate = 1f; // quanto drena por segundo

    public bool IsEmpty => currentCharge <= 0;
   

    void Awake()
    {
        currentCharge = maxCharge;
    }

    public void Drain(float amount)
    {
        currentCharge -= amount;
        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);

        if (IsEmpty)
        {
            //morte
        }
    }

    public void Recharge(float amount)
    {
        currentCharge += amount;
        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
    }

    private void DrainOverTime()
    {
        if (!IsEmpty )
        {
            currentCharge -= drainRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);

            if (IsEmpty)
            {
               //morte
            }
        }
    }

    public void FullRecharge()
    {
        currentCharge = maxCharge;
    }
}