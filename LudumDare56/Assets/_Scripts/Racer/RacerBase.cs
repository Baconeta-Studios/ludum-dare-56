using System;
using System.Collections;
using UnityEngine;

public class RacerBase : MonoBehaviour
{
    private Track track;
    private Rigidbody2D racerRigidbody2d; 
    
    [Header("Card Components")]
    [SerializeField] private BoostComponent boost;
    [SerializeField] private BrakeComponent brake;

    [Header("Lap Progress")] 
    [SerializeField] [ReadOnly] private float distanceAlongTrack;
    public float DistanceAlongTrack => distanceAlongTrack;
    private Vector3 positionOnTrackSpline;
    private Vector3 tangentOnTrackSpline;
    
    [Header("Movement")]
    [SerializeField] [ReadOnly] private float currentSpeed;
    private Vector3 currentHeading;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 1f;

    [Header("Whiskers")] 
    [SerializeField] private float forwardsWhiskerLength;
    [SerializeField] private float sideWhiskerLength;
    [SerializeField] private float sideWhiskerAngle;
    [SerializeField] private float sideWhiskerTurnAnglePerSecond;
    private Vector3 whiskerFront;
    private Vector3 whiskerRight;
    private Vector3 whiskerLeft;

    [Header("Alignment")] 
    [SerializeField] private float alignmentTurnAnglePerSecond = 5f;
    
    [Header("Respawning")] 
    [ReadOnly] public bool isRespawning;
    public float respawnStartDelay;
    public float respawnDuration;
    
    [Header("Debug")]
    public float positionGizmoRadius = 1f;
        
    private void Start()
    {
        track = FindFirstObjectByType<Track>();
        racerRigidbody2d = GetComponent<Rigidbody2D>();
        
        BeginRace();
    }

    private void BeginRace()
    {
        currentHeading = transform.up;
    }

    private void Update()
    {
        UpdateTrackPosition();
    }

    private void UpdateTrackPosition()
    {
        distanceAlongTrack = track.GetDistanceToSpline(transform.position, out Vector3 positionOnSpline, out Vector3 tangentOnSpline);
        positionOnTrackSpline = positionOnSpline;
        tangentOnTrackSpline = tangentOnSpline;
    }

    private void FixedUpdate()
    {
        if (!isRespawning)
        {
            MovementUpdate();
        }
    }

    private void MovementUpdate()
    {
        // Whiskers
        whiskerFront = transform.up * forwardsWhiskerLength;
        // Setup Whisker Right
        Vector3 sideWhiskerDirectionRight = Vector3.RotateTowards(transform.up, transform.right, Mathf.Deg2Rad * sideWhiskerAngle, 0);
        whiskerRight = sideWhiskerDirectionRight * sideWhiskerLength;
        //Setup Whisker Left
        Vector3 sideWhiskerDirectionLeft = Vector3.RotateTowards(transform.up, transform.right * -1, Mathf.Deg2Rad * sideWhiskerAngle, 0);
        whiskerLeft = sideWhiskerDirectionLeft * sideWhiskerLength;

        AlignWithTrack();
        CheckSideWhisker(sideWhiskerDirectionLeft, -1);
        CheckSideWhisker(sideWhiskerDirectionRight, 1);

        if (brake.IsActive && currentSpeed > brake.MaxSpeed)
        {
            currentSpeed -= brake.Deceleration * Time.fixedDeltaTime;
        }
        else if (boost.IsFinishing) // Boost - Finished, so decelerate to max speed.
        {
            currentSpeed -= boost.Deceleration * Time.fixedDeltaTime;
            if (currentSpeed <= maxSpeed)
            {
                boost.OverrideFinished();
            }
        }
        else // Acceleration
        {
            bool isBoosting = boost.IsActive;
            float accelerationChange = isBoosting ? boost.Acceleration : acceleration;

            if (currentSpeed <= (isBoosting ? boost.MaxSpeed : maxSpeed))
            {
                currentSpeed += accelerationChange * Time.fixedDeltaTime;
            }
        }

        racerRigidbody2d.MovePosition(transform.position + currentHeading * currentSpeed);
        transform.up = currentHeading;
    }

    private void AlignWithTrack()
    {
        currentHeading = Vector3.RotateTowards(currentHeading, tangentOnTrackSpline,
            (Mathf.Deg2Rad * alignmentTurnAnglePerSecond) * Time.fixedDeltaTime,
            0);
    }

    private void CheckSideWhisker(Vector3 whiskerDirection, int side)
    {
        var hits = Physics2D.RaycastAll(transform.position, whiskerDirection, sideWhiskerLength);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.transform.CompareTag("Track"))
                {
                    // Collided with the track border.
                    currentHeading = Vector3.RotateTowards(currentHeading, transform.right * -side,
                        (Mathf.Deg2Rad * sideWhiskerTurnAnglePerSecond) * Time.fixedDeltaTime,
                        0);
                    break;
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isRespawning && collision.transform.CompareTag("Track"))
        {
            // We've left the track. Time to respawn.
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        isRespawning = true;
        
        boost.RacerRespawned();
        brake.RacerRespawned();
        
        yield return new WaitForSeconds(respawnStartDelay);
        transform.position = positionOnTrackSpline;
        
        // Face the splines tangent, and also reset the heading + speed.
        transform.up = tangentOnTrackSpline;
        racerRigidbody2d.velocity = Vector3.zero;
        racerRigidbody2d.angularVelocity = 0f;
        currentHeading = transform.up;
        currentSpeed = 0f;
        
        yield return new WaitForSeconds(respawnDuration);
        
        isRespawning = false;
        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(positionOnTrackSpline, positionGizmoRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (currentSpeed * currentHeading));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + whiskerFront);
        Gizmos.DrawLine(transform.position , transform.position + whiskerRight);
        Gizmos.DrawLine(transform.position, transform.position + whiskerLeft);
    }
}
