using _Scripts.Racer;
using UnityEngine;
using UnityEngine.Serialization;

public class RacerAi : RacerBase
{
    private float cardUpdateInterval = 0.3f;
    private float timeOfNextCardUpdate;
    [SerializeField] private float cardCooldownDuration;
    [SerializeField] private float cardCooldownFinishTime;
    
    [Header("Ai Brake")]
    [SerializeField] private float brakeRaycastDistance = 20f;
    [SerializeField] private float brakeTangentLimit = 0f;
    [SerializeField] private float maxDistanceForBrake = 50f;
    
    [Header("Ai Boost")]
    [SerializeField] private float boostRaycastDistance;
    [SerializeField] private float boostTangentThreshold = 0.5f;
    [SerializeField] private float minDistanceForBoost = 50f;
    protected override void Update()
    {
        base.Update();
        if (Time.time > timeOfNextCardUpdate)
        {
            timeOfNextCardUpdate = Time.time + cardUpdateInterval;
            TryUseBoost();
            TryUseBreak();
        }
        
    }

    private void TryUseBreak()
    {
        var distToSpline = GetSplineInfoAtDistance(brakeRaycastDistance, out var nearestSpline, out var tangentOnSpline);

        if (distToSpline <= maxDistanceForBrake)
        {
            float trackTangentRacerDot = Vector3.Dot(tangentOnSpline, transform.up) ;

            if (trackTangentRacerDot < brakeTangentLimit)
            {
                TryUseCard<BrakeCard>();
            }
        }
    }

    private void TryUseBoost()
    {
        var distToSpline = GetSplineInfoAtDistance(boostRaycastDistance, out var nearestSpline, out var tangentOnSpline);

        if (distToSpline >= minDistanceForBoost)
        {
            float trackTangentRacerDot = Vector3.Dot(tangentOnSpline, transform.up) ;
            
            if (trackTangentRacerDot >= boostTangentThreshold)
            {
                // Use Boost Card (If Available)
                TryUseCard<BoostCard>();
            }
        }
    }

    private void TryUseCard<T>()
    {
        if (deck && deck.Hand != null)
        {
            foreach (var card in deck.Hand)
            {
                if (card is T typedCard)
                {
                    // Successfully cast to BoostCard, so use boostCard here
                    deck.PlayCard(card, null);
                    break;
                }
            }
        }
    }

    private float GetSplineInfoAtDistance(float rayDist, out Vector3 nearestSpline, out Vector3 tangentOnSpline)
    {
        track.GetDistanceToSpline(
            transform.position + transform.up * rayDist,
            out nearestSpline,
            out tangentOnSpline);

        float distToSpline = Vector2.Distance(transform.position, nearestSpline);
        return distToSpline;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && collision.CompareTag("ShortcutPrep"))
        {
            TryUseCard<ShortcutCard>();
        }
    }
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * boostRaycastDistance);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * brakeRaycastDistance);
        Gizmos.color = Color.red;

    }
}
