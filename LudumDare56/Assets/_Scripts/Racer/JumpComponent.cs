using System.Collections;
using System.Collections.Generic;
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
    void Update()
    {
        if (isActive)
        {
            var hits = Physics2D.RaycastAll(transform.position, transform.up, jumpDetectionDistance);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("SabotageObject"))
                {
                    StartCoroutine(EJump(Vector2.Distance(hit.transform.position, transform.position)));
                }
            }
        }
    }

    IEnumerator EJump(float distanceToJump)
    {
        isActive = false;
        isJumping = true;
        racer.UseActiveCard();
        racer.DisableCollision();
        float timeToClearJump = (distanceToJump / racer.CurrentSpeed) * 2;
        float t = 0;
        Vector3 originalSize = transform.localScale;
        
        while (t < timeToClearJump)
        {
            transform.localScale = Vector3.Lerp(originalSize, originalSize * jumpScalePercent, jumpAnimationCurve.Evaluate(t / timeToClearJump));
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalSize;
        
        racer.EnableCollision();
        isJumping = false;
        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * jumpDetectionDistance);
    }
}
