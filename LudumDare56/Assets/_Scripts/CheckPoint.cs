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
                    OnRacerCrossCheckPoint?.Invoke(racer);
                }
            }
        }

        public void JumpingRacerCrossedCheckpoint(RacerBase racer)
        {
            if (racer)
            {
                OnRacerCrossCheckPoint?.Invoke(racer);
            }
        }
    }
}