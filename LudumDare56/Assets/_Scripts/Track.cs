using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Track : MonoBehaviour
{
    private SplineContainer trackSpline;
    [SerializeField] private float trackWidth;

    private void Awake()
    {
        trackSpline = GetComponent<SplineContainer>();
    }
    public float GetDistanceToSpline(Vector3 position, out Vector3 nearestPointOnSpline, out Vector3 tangentOnSpline)
    {
        SplineUtility.GetNearestPoint(trackSpline.Spline, position, out float3 nearestPoint, out float distanceAlongTrack);
        tangentOnSpline = trackSpline.Spline.EvaluateTangent(distanceAlongTrack);
        nearestPointOnSpline = nearestPoint;
        
        return distanceAlongTrack;
    }
}
