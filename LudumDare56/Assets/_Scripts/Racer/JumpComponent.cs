using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Racer;
using UnityEngine;

public class JumpComponent : MonoBehaviour
{    
    private RacerBase racer;
    [SerializeField] private bool isActive;
    [SerializeField] private bool isJumping;
    [SerializeField] private float jumpDetectionDistance;
    [SerializeField] private AnimationCurve jumpAnimationCurve;
    [SerializeField] private float jumpScalePercent;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private float jumpSoundVolume;

    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }

    public bool IsJumping => isJumping;
    
    
    private void Awake()
    {
        racer = GetComponent<RacerBase>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isActive)
        {
            var hits = Physics2D.RaycastAll(transform.position, transform.up, jumpDetectionDistance);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("SabotageObject"))
                {
                    StartCoroutine(EJump(Vector2.Distance(hit.transform.position, transform.position), hit.collider));
                }
            }
        }
    }

    private IEnumerator EJump(float distanceToJump, Collider2D hitCollider)
    {
        isActive = false;
        isJumping = true;
        racer.UseActiveCard();
        if (jumpSound != null)
        {
            AudioSystem.Instance.PlaySound(jumpSound, jumpSoundVolume);
        }

        racer.DisableCollision();
        
        float timeToReachJump = distanceToJump / racer.CurrentSpeed;
        float timeToClearObstacle = (4 / racer.CurrentSpeed) + ((CircleCollider2D)hitCollider).radius * hitCollider.transform.localScale.magnitude / racer.CurrentSpeed;
        float timeToEndJump = timeToReachJump + timeToClearObstacle;
        float t = 0;
        Vector3 originalSize = transform.localScale;
        bool hitCheckpointOrFinish = false;
        while (t < timeToEndJump)
        {
            transform.localScale = Vector3.Lerp(originalSize, originalSize * jumpScalePercent, jumpAnimationCurve.Evaluate(t / timeToEndJump));
            t += Time.deltaTime;

            if (!hitCheckpointOrFinish)
            {
                hitCheckpointOrFinish = CheckForFinishOrCheckpoint();
            }
            yield return null;
        }

        transform.localScale = originalSize;
        
        racer.EnableCollision();
        isJumping = false;
        yield return null;
    }

    private bool CheckForFinishOrCheckpoint()
    {
        // Band-aid for having no collision over finish line and checkpoints
        var hits = Physics2D.OverlapBoxAll(transform.position, Vector2.one, 0);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Finish"))
            {
                hit.GetComponent<FinishLine>().JumpingRacerCrossedFinishLine(GetComponent<RacerBase>());
                hit.GetComponent<CheckPoint>().JumpingRacerCrossedCheckpoint(GetComponent<RacerBase>());
                return true;
            }
            else if (hit.CompareTag("Checkpoint"))
            {
                hit.GetComponent<CheckPoint>().JumpingRacerCrossedCheckpoint(GetComponent<RacerBase>());
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * jumpDetectionDistance);
    }
}
