using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RacerBase : MonoBehaviour
{
    private Track track;
    [SerializeField] private float distanceAlongTrack;
    private Vector3 positionOnTrackSpline;
    
    [Header("Debug")]
    public float positionGizmoRadius;
    
    public void Start()
    {
        track = FindFirstObjectByType<Track>();
    }

    public void Update()
    {
        distanceAlongTrack = track.GetDistanceToSpline(transform.position, out Vector3 positionOnSpline);
        positionOnTrackSpline = positionOnSpline;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawSphere(positionOnTrackSpline, positionGizmoRadius);
    }
}
