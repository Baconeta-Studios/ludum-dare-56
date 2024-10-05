using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostComponent : MonoBehaviour
{
    [SerializeField] private ParticleSystem boostEffect;
    [SerializeField] [ReadOnly] private bool isBoosting;
    [SerializeField] [ReadOnly] private bool isFinishingBoost;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float boostingMaxSpeed = 0.8f;
    [SerializeField] private float boostingAcceleration = 0.5f;
    [SerializeField] private float boostDeceleration = 0.3f;
    [SerializeField] [ReadOnly] private float boostTimeRemaining = 0f;
    
    public bool IsBoosting => isBoosting;
    
    public bool IsFinishingBoost => isFinishingBoost;
    
    public float BoostTimeRemaining => boostTimeRemaining;
    
    public float BoostingAcceleration => boostingAcceleration;
    
    public float BoostDeceleration => boostDeceleration;

    public float BoostMaxSpeed => boostingMaxSpeed;

    private void Update()
    {
        if (isBoosting)
        {
            boostTimeRemaining -= Time.deltaTime;
            if (boostTimeRemaining <= 0)
            {
                EndBoost();
            }
        }
    }
    
    [ContextMenu("Boost")]
    public void StartBoost()
    {
        if (isBoosting)
        {
            boostTimeRemaining += boostDuration;
        }
        else
        {
            boostTimeRemaining = boostDuration;
        }

        isBoosting = true;
        boostEffect.Play();
    }

    private void EndBoost(bool forceFinish = false)
    {
        isBoosting = false;
        boostTimeRemaining = 0f;

        if (forceFinish)
        {
            BoostFinished();
        }
        else
        {
            isFinishingBoost = true;
        }
    }

    public void BoostFinished()
    {
        isFinishingBoost = false;
        boostEffect.Stop();
    }

    public void RacerRespawned()
    {
        if (isBoosting)
        {
            EndBoost(true);
        }
    }


}
