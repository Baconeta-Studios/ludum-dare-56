using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Track : MonoBehaviour
{
    [SerializeField] private SplineContainer trackSpline;
    [SerializeField] private float trackWidth;
    public float TrackWidth => trackWidth;
    [SerializeField] [Range(0,1)] private float positionOnTrackToShowWidth;

    private void Awake()
    {
        trackSpline = GetComponent<SplineContainer>();
    }
    public float GetDistanceToSpline(Vector3 position, out Vector3 nearestPointOnSpline, out Vector3 tangentOnSpline)
    {
        SplineUtility.GetNearestPoint(trackSpline.Spline, position, out float3 nearestPoint, out float distanceAlongTrack);
        tangentOnSpline = trackSpline.Spline.EvaluateTangent(distanceAlongTrack);
        tangentOnSpline.z = 0;
        tangentOnSpline.Normalize();
        
        nearestPointOnSpline = nearestPoint;
        
        return distanceAlongTrack;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (trackSpline)
        {
            Vector3 trackPosition = trackSpline.EvaluatePosition(positionOnTrackToShowWidth);
            Gizmos.DrawLine(trackPosition + new Vector3(trackWidth/2,0,0), trackPosition + new Vector3(-trackWidth/2,0,0));
        }

    }
}
