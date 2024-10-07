using Utils;

namespace _Scripts.Managers
{
    public class PlayerStatsManager : Singleton<PlayerStatsManager>
    {
        public static string PlayerUsername = "Rustle Jr.";
        
        private static void PrepareScoreSubmission(int trackTime, int bestLapTime)
        {
            var ssm = ScoreServerManager.Instance;
            ssm.SubmitScore(ScoreServerManager.ScoreType.Track, PlayerUsername, trackTime);
            ssm.SubmitScore(ScoreServerManager.ScoreType.Lap, PlayerUsername, bestLapTime);
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