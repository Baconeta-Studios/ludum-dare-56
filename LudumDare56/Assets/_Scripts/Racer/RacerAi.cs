using _Scripts;
using _Scripts.Racer;
using UnityEngine;
using Random = UnityEngine.Random;

public class RacerAi : RacerBase
{
    private float cardUpdateInterval = 0.3f;
    private float timeOfNextCardUpdate;
    [SerializeField] private float cardCooldownDuration;
    [SerializeField] private float cardCooldownFinishTime;
    [SerializeField] private float chanceToDrawTwoCards = 0.4f;
    
    [Header("Ai Brake")]
    [SerializeField] private float brakeRaycastDistance = 20f;
    [SerializeField] private float brakeTangentLimit = 0f;
    [SerializeField] private float maxDistanceForBrake = 50f;
    
    [Header("Ai Boost")]
    [SerializeField] private float boostRaycastDistance;
    [SerializeField] private float boostTangentThreshold = 0.5f;
    [SerializeField] private float minDistanceForBoost = 50f;
    
    [Header("Ai Jump")]
    [SerializeField] private float jumpRaycastDistance = 10f;

    [Header("Ai Sabotage")] 
    [SerializeField] private float sabotageChance;
    [SerializeField] private float sabotageInterval;
    private float nextSabotageCheckTime;

    protected override void OnEnable()
    {
        base.OnEnable();
        CheckPoint.OnRacerCrossCheckPoint += OnRacerCrossCheckPoint;
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        CheckPoint.OnRacerCrossCheckPoint -= OnRacerCrossCheckPoint;
    }

    private void OnRacerCrossCheckPoint(RacerBase racer)
    {
        if (racer == this)
        {
            deck.DrawCard();
            if (Random.Range(0f, 1f) <= chanceToDrawTwoCards);
            {
                deck.DrawCard();
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!RaceManager.Instance.HasRaceStarted) return;
        
        if (Time.time > timeOfNextCardUpdate)
        {
            timeOfNextCardUpdate = Time.time + cardUpdateInterval;
            
            TryUseBoost();
            
            TryUseBrake();

            TryUseJump();

            if (Time.time > nextSabotageCheckTime)
            {
                nextSabotageCheckTime = Time.time + sabotageInterval;
                TryUseSabotage();
            }
        }
        
    }

    private void TryUseSabotage()
    {
        float random = Random.Range(0f, 1f);

        if (random <= sabotageChance)
        {
            TryUseCard<SabotageCard>();
        }
    }

    private void TryUseJump()
    {
        var hits = Physics2D.RaycastAll(transform.position, Vector3.up, brakeRaycastDistance);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("SabotageObject"))
            {
                TryUseCard<JumpCard>();
                break;
            }
        }
    }

    private void TryUseBrake()
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

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
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
        Gizmos.DrawLine(transform.position, transform.position + transform.up * jumpRaycastDistance);
        

    }
}
