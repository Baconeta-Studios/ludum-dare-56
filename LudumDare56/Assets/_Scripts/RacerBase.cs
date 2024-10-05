using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RacerBase : MonoBehaviour
{
    private Track track;
    private Rigidbody2D racerRigidbody2d;
    [Header("Lap Progress")]
    [SerializeField] private float distanceAlongTrack;
    private Vector3 positionOnTrackSpline;
    private Vector3 tangentOnTrackSpline;
    
    [Header("Movement")]
    private Vector3 currentHeading;
    [SerializeField] [ReadOnly] private float currentSpeed;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 1f;

    [Header("Boost")] 
    [SerializeField] private ParticleSystem boostEffect;
    [SerializeField] private bool isBoosting;
    [SerializeField] private bool isFinishingBoost;
    [SerializeField] private float boostDuration = 1f;
    [SerializeField] private float boostingMaxSpeed = 1f;
    [SerializeField] private float boostingAcceleration = 1f;
    [SerializeField] private float boostDeceleration = 1f;
    private float boostTimeRemaining = 1f;

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
    public bool isRespawning;
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

        if (isBoosting)
        {
            boostTimeRemaining -= Time.deltaTime;
            if (boostTimeRemaining <= 0)
            {
                EndBoost();
            }
        }
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
        

        // Boost - Finished, so decelerate to max speed.
        if (isFinishingBoost)
        {
            currentSpeed -= boostDeceleration * Time.fixedDeltaTime;
            if (currentSpeed <= maxSpeed)
            {
                BoostFinished();
            }
        }
        else // Acceleration
        {
            float accelerationToAdd = isBoosting ? boostingAcceleration : acceleration;

            if (currentSpeed <= (isBoosting ? boostingMaxSpeed : maxSpeed))
            {
                currentSpeed += accelerationToAdd * Time.fixedDeltaTime;
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

    private void BoostFinished()
    {
        isFinishingBoost = false;
        boostEffect.Stop();
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

        if (isBoosting)
        {
            EndBoost(true);
        }
        
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
