using System;
using _Scripts.Racer;
using UnityEngine;
using Utils;

namespace _Scripts.Managers
{
    public class GameTimeManager : Singleton<GameTimeManager>
    {
        private bool isRacing;
        
        // Total time since the race began, updated each Update().
        public float totalRaceTimeSoFar;
        // Current Lap time.
        private float totalLapTimeSoFar;
        // Fastest Lap.
        public float fastestLap;
        // This flag ensures that our first lap always becomes our best lap.
        private bool fastestLapExists = false;
        
        private float lastTimeFinishLineCrossed; // Used to calculate lap times.
        
        public static event Action<float> OnRaceTimeChanged;
        public static event Action<float> OnLapTimeChanged;
        public static event Action<float> OnFastestLapTimeChanged;
        public static event Action<float, float> OnRaceFinished;
        
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
            HandleLapEndEvent(FindFirstObjectByType<RacerPlayer>());
            isRacing = false;
            OnRaceFinished?.Invoke(totalRaceTimeSoFar, totalLapTimeSoFar);
            Debug.Log($"Fastest Lap: {fastestLap}\nRace Time: {totalRaceTimeSoFar}");
        }

        public void HandleLapEndEvent(RacerBase racer)
        {
            // We only care about the player completing a lap. AI lap completion is handled elsewhere.
            if (racer.GetType() != typeof(RacerPlayer))
            {
                return;
            }
            
            // The first time that the player completes a lap, start the race and do nothing else.
            if (!isRacing && !RaceManager.Instance.HasRaceFinished)
            {
                StartRaceTimer();
                return;
            }
            
            // Update fastest-lap time.
            if (!fastestLapExists || totalLapTimeSoFar < fastestLap)
            {
                fastestLap = totalLapTimeSoFar;
                fastestLapExists = true;
                OnFastestLapTimeChanged?.Invoke(fastestLap);
            }

            // Reset lap timer.
            lastTimeFinishLineCrossed = totalRaceTimeSoFar;
        }

        private void OnEnable()
        {
            FinishLine.OnRacerCrossFinishLine += HandleLapEndEvent;
            RaceManager.OnRaceCompleted += StopRaceTimer;
        }

        private void OnDisable()
        {
            FinishLine.OnRacerCrossFinishLine -= HandleLapEndEvent;
            RaceManager.OnRaceCompleted -= StopRaceTimer;

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