using System;
using _Scripts.Racer;
using UnityEngine;

namespace _Scripts.Managers
{
    public class GameTimeManager : MonoBehaviour
    {
        private bool isRacing;
        
        private float totalRaceTimeSoFar; // Total time since the race began, updated each Update().
        // Current Lap time.
        private float lapTimeSoFar;
        private float lastTimeFinishLineCrossed; // Used to calculate lap times.
        // Fastest Lap.
        private float fastestLap;
        
        public static event Action<float> OnRaceTimeChanged;
        public static event Action<float> OnLapTimeChanged;
        public static event Action<float> OnFastestLapTimeChanged;
        
        public void StartRaceTimer()
        {
            // Reset timer.
            totalRaceTimeSoFar = 0;
            lastTimeFinishLineCrossed = 0;
            lapTimeSoFar = 0;
            fastestLap = 0;
            
            // Start timer.
            isRacing = true;
        }

        public void StopRaceTimer()
        {
            isRacing = false;
        }

        public void HandleLapEndEvent(RacerBase racer)
        {
            if (!isRacing)
            {
                StartRaceTimer();
                return;
            }
            // Calculate last-lap time.
            lapTimeSoFar = totalRaceTimeSoFar - lastTimeFinishLineCrossed;
            lastTimeFinishLineCrossed = totalRaceTimeSoFar;
            
            // Update fastest-lap time.
            if (lapTimeSoFar < fastestLap)
            {
                fastestLap = lapTimeSoFar;
            }
            
            
            OnFastestLapTimeChanged?.Invoke(fastestLap * 1_000);
        }

        private void OnEnable()
        {
            FinishLine.OnRacerCrossFinishLine += HandleLapEndEvent;
        }

        private void OnDisable()
        {
            FinishLine.OnRacerCrossFinishLine -= HandleLapEndEvent;
        }
        
        private void Update()
        {
            if (!isRacing) return;
            
            totalRaceTimeSoFar += Time.deltaTime;
            OnRaceTimeChanged?.Invoke(totalRaceTimeSoFar);
            OnLapTimeChanged?.Invoke(lapTimeSoFar);
        }
    }
}