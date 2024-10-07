using System.Collections;
using _Scripts.Racer;
using UnityEngine;

public class ShortcutComponent : MonoBehaviour
{
    private RacerBase racer;
    [SerializeField] private bool isActive;
    [SerializeField] private bool isInShortcut;
    
    private TrackShortcut currentTrackShortcut;
    [SerializeField] private float minDistanceForActions = 1f;
    
    [SerializeField] private float sizeCurveDuration;
    [SerializeField] private AnimationCurve sizeCurve;
    
    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }

    public bool IsInShortcut => isInShortcut;

    private void Awake()
    {
        racer = GetComponent<RacerBase>();
    }
    
    private void OnTriggerEnter2D(Collider2D collider2d)
    {
        if (isActive)
        {
            if(collider2d.transform)
            {
                if (collider2d.CompareTag("Shortcut"))
                {
                    TrackShortcut trackShortcut = collider2d.transform.parent.GetComponent<TrackShortcut>();
                    ShortcutDetected(trackShortcut);
                }
            }
        }
    }

    private void ShortcutDetected(TrackShortcut trackShortcut)
    {
        currentTrackShortcut = trackShortcut;
        isInShortcut = true;
        
        StartCoroutine(UseShortcut());
    }

    private IEnumerator UseShortcut()
    {
        racer.UseActiveCard();
        racer.DisableCollision();
        float angle;
        
        // Travel to shortcut start
        Vector3 endPos = currentTrackShortcut.ShortcutStart.position;
        while (Vector3.Distance(transform.position, endPos) > minDistanceForActions)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, racer.CurrentSpeed * Time.deltaTime);
            Vector2 directionToStart = endPos - transform.position;
            angle = Mathf.Atan2(directionToStart.y, directionToStart.x) * Mathf.Rad2Deg;
            
            transform.rotation = Quaternion.Euler(new Vector3(0,0, angle - 90));
            yield return null;
        }
        
        float t = 0;
        
        Vector3 originalLocalScale = transform.localScale;
        // Do a small animation/shrink of the racer going into the hole
        while (t < sizeCurveDuration)
        {
            transform.localScale = Vector3.Lerp(originalLocalScale, Vector3.zero, sizeCurve.Evaluate(t / sizeCurveDuration));
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.zero;

        // Travel from start to finish
        Vector3 shortCutStartPos = currentTrackShortcut.ShortcutStart.position;
        Vector3 shortCutEndPos = currentTrackShortcut.ShortcutEnd.position;
        t = 0;
        while (t < currentTrackShortcut.ShortcutDuration)
        {
            transform.position = Vector3.Lerp(
                shortCutStartPos,
                shortCutEndPos,
                t / currentTrackShortcut.ShortcutDuration);

            Vector2 directionOfTravel = (shortCutStartPos - transform.position).normalized;
            angle = Mathf.Atan2(directionOfTravel.y, directionOfTravel.x) * Mathf.Rad2Deg;
            
            transform.rotation = Quaternion.Euler(new Vector3(0,0, angle - 90));
            
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = shortCutEndPos;
        
        // Face exit heading
        Vector2 directionOfHeading = ((shortCutEndPos + (Vector3)currentTrackShortcut.ShortcutEndHeading) - transform.position).normalized;
        angle = Mathf.Atan2(directionOfHeading.y, directionOfHeading.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0,0, angle - 90));
        
        // Do a small animation/shrink of the racer going into the hole
        t = 0f;
        while (t < sizeCurveDuration)
        {
            transform.localScale = Vector3.Lerp(originalLocalScale, Vector3.zero, sizeCurve.Evaluate(1 - (t / sizeCurveDuration)));
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalLocalScale;
        
        // Exit shortcut
        isInShortcut = false;
        isActive = false;
        racer.ExitedShortcut(currentTrackShortcut.ShortcutEndHeading);
        
        yield return null;
    }
}
