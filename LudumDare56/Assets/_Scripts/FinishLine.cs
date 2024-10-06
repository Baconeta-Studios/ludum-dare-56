using System;
using _Scripts.Racer;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public static event Action<RacerBase> OnRacerCrossFinishLine;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform && collision.transform.parent)
        {
            var racer = collision.transform.parent.GetComponent<RacerBase>();
            if (racer)
            {
                Debug.Log($"Racer Crossed Finish Line: {racer.name}");
                OnRacerCrossFinishLine?.Invoke(racer);
            }
        }
    }
}
