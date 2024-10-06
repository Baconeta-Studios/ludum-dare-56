using UnityEngine;

public class SpeedOverrideComponent : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleEffect;
    [SerializeField] [ReadOnly] private bool isActive;
    [SerializeField] [ReadOnly] private bool isFinishing;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float maxSpeed = 0.8f;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float deceleration = 0.3f;
    [SerializeField] [ReadOnly] private float timeRemaining = 0f;
    
    public bool IsActive => isActive;
    
    public bool IsFinishing => isFinishing;
    
    public float TimeRemaining => timeRemaining;
    
    public float Acceleration => acceleration;
    
    public float Deceleration => deceleration;

    public float MaxSpeed => maxSpeed;

    private void Update()
    {
        if (isActive)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                EndOverride();
            }
        }
    }
    
    [ContextMenu("Activate Override")]
    public virtual void StartOverride()
    {
        if (isActive)
        {
            timeRemaining += duration;
        }
        else
        {
            timeRemaining = duration;
        }

        isActive = true;
        particleEffect.Play();
    }

    protected virtual void EndOverride(bool forceFinish = false)
    {
        isActive = false;
        timeRemaining = 0f;

        if (forceFinish)
        {
            OverrideFinished();
        }
        else
        {
            isFinishing = true;
        }
    }

    public void OverrideFinished()
    {
        isFinishing = false;
        particleEffect.Stop();
    }

    public void RacerRespawned()
    {
        if (isActive)
        {
            EndOverride(true);
        }
    }


}
