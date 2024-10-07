using UnityEngine;
using Utils;

namespace _Scripts.Managers
{
    public class PlayerStatsManager : Singleton<PlayerStatsManager>
    {
        public static string defaultUsername = "Rustle Jr.";

        private static string GetPlayerUsername()
        {
            string playerUsername = PlayerPrefs.GetString("PlayerName");

            if (playerUsername != null && playerUsername.Length > 0)
            {
                return playerUsername;
            }
            
            // Change the return if you'd desire a generated name instead of default.
            return defaultUsername;
            
        }
        private static void PrepareScoreSubmission(float trackTime, float bestLapTime)
        {
            var ssm = ScoreServerManager.Instance;
            ssm.SubmitScore(ScoreServerManager.ScoreType.Track, GetPlayerUsername(), trackTime);
            ssm.SubmitScore(ScoreServerManager.ScoreType.Lap, GetPlayerUsername(), bestLapTime);
        }
        
        private void OnEnable()
        {
            GameTimeManager.OnRaceFinished += PrepareScoreSubmission;
        }

        private void OnDisable()
        {
            GameTimeManager.OnRaceFinished -= PrepareScoreSubmission;
        }
    }
}