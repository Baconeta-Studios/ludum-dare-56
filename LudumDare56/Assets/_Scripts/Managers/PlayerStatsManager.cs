using Utils;

namespace _Scripts.Managers
{
    public class PlayerStatsManager : Singleton<PlayerStatsManager>
    {
        public static string PlayerUsername = "Rustle Jr.";
        
        private static void PrepareScoreSubmission(float trackTime, float bestLapTime)
        {
            var ssm = ScoreServerManager.Instance;
            ssm.SubmitScore(ScoreServerManager.ScoreType.Track, PlayerUsername, trackTime.ToString("en-GB"));
            ssm.SubmitScore(ScoreServerManager.ScoreType.Lap, PlayerUsername, bestLapTime.ToString("en-GB"));
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