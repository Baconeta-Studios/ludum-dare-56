using System;
using _Scripts.Racer;
using UnityEngine;

namespace _Scripts
{
    public class CheckPoint : MonoBehaviour
    {
        public static event Action<RacerBase> OnRacerCrossCheckPoint;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform && collision.transform.parent)
            {
                var racer = collision.transform.parent.GetComponent<RacerBase>();
                if (racer)
                {
                    Debug.Log($"Racer Crossed Checkpoint: {racer.name}");
                    OnRacerCrossCheckPoint?.Invoke(racer);
                }
            }
        }
    }
}