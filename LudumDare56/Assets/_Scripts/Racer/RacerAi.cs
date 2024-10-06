using _Scripts.Racer;
using UnityEngine;

public class RacerAi : RacerBase
{
    [SerializeField] private float boostRaycastDistance;
    [SerializeField] private float brakeRaycastDistance;

    private void Update()
    {
        var hits = Physics2D.RaycastAll(transform.position, transform.forward, boostRaycastDistance);
        bool canBoost = true;
        foreach (var hit in hits)
        {
            if (hit.collider && hit.transform.CompareTag("Track"))
            {
                // Hit a track wall
                canBoost = false;
            }
        }
        // Use Boost Card (If Available)
        if (canBoost && deck && deck.Hand != null)
        {
            foreach (var card in deck.Hand)
            {
                BoostCard boostCard = card as BoostCard;
                if (boostCard != null)
                {
                    // Successfully cast to BoostCard, so use boostCard here
                    boostCard.TryUseCard();
                    break;
                }
            }
        }
        
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * boostRaycastDistance);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * brakeRaycastDistance);

    }
}
