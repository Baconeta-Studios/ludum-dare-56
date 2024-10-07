using System;
using _Scripts.Racer;
using UnityEngine;

namespace _Scripts.Managers
{
    public class GameTimeManager : MonoBehaviour
    {
        private bool isRacing;
        
        // Total time since the race began, updated each Update().
        private float totalRaceTimeSoFar;
        // Current Lap time.
        private float totalLapTimeSoFar;
        // Fastest Lap.
        private float fastestLap;
        
        private float lastTimeFinishLineCrossed; // Used to calculate lap times.
        
        public static event Action<float> OnRaceTimeChanged;
        public static event Action<float> OnLapTimeChanged;
        public static event Action<float> OnFastestLapTimeChanged;
        
        public void StartRaceTimer()
        {
            // Reset timer.
            totalRaceTimeSoFar = 0;
            totalLapTimeSoFar = 0;
            fastestLap = 0;
            lastTimeFinishLineCrossed = 0;
            
            // Start timer.
            isRacing = true;
        }

        public void StopRaceTimer()
        {
            isRacing = false;
        }

        public void HandleLapEndEvent(RacerBase racer)
        {
            if (racer.GetType() != typeof(RacerPlayer))
            {
                return;
            }
            
            if (!isRacing)
            {
                StartRaceTimer();
                return;
            }
            // Calculate last-lap time.
            lapTimeSoFar = totalRaceTimeSoFar - lastTimeFinishLineCrossed;
            lastTimeFinishLineCrossed = totalRaceTimeSoFar;
            
            // Update fastest-lap time.
            if (totalLapTimeSoFar > fastestLap)
            {
                fastestLap = totalLapTimeSoFar;
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
            totalLapTimeSoFar = totalRaceTimeSoFar - lastTimeFinishLineCrossed;
            OnRaceTimeChanged?.Invoke(totalRaceTimeSoFar);
            OnLapTimeChanged?.Invoke(totalLapTimeSoFar);
        }
    }
}