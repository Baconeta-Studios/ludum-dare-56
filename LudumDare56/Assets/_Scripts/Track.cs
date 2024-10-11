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
            // Evaluate position on the track
            Vector3 trackPosition = trackSpline.EvaluatePosition(positionOnTrackToShowWidth);
        
            // Evaluate tangent (direction) at the position
            Vector3 trackTangent = trackSpline.EvaluateTangent(positionOnTrackToShowWidth);
        
            // Normalize tangent to get the direction
            Vector3 trackRight = Vector3.Cross(trackTangent, Vector3.forward).normalized;
        
            // Draw line based on the tangent direction
            Gizmos.DrawLine(trackPosition + trackRight * (trackWidth / 2), trackPosition - trackRight * (trackWidth / 2));
        }

    }
}
