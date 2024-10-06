using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TrackShortcut : MonoBehaviour
{
    [SerializeField] private Transform shortcutStart;
    [SerializeField] private Transform shortcutEnd;
    [FormerlySerializedAs("shortcutEndFacing")] [SerializeField] private Vector2 shortcutEndHeading;

    [SerializeField] private float shortcutDuration;

    public Transform ShortcutStart => shortcutStart;

    public Transform ShortcutEnd => shortcutEnd;
    
    public float ShortcutDuration => shortcutDuration;
    
    public Vector2 ShortcutEndHeading => shortcutEndHeading;

    [Header("Debug")] 
    [SerializeField] private float shortCutExitArrowLength = 10f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Gizmos.DrawLine(shortcutStart.position, shortcutEnd.position);
        Gizmos.DrawLine(shortcutEnd.position, shortcutEnd.position + (Vector3)shortcutEndHeading * shortCutExitArrowLength);

    }
}
