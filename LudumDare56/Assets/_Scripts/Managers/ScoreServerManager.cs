using System;
using System.Collections;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace _Scripts.Managers
{
    public class ScoreServerManager : MonoBehaviour
    {
        [Header("Track Text Fields")]
        
        [Tooltip("High-score 'Name' Column")] [SerializeField]
        private TextMeshProUGUI nameColumnTrack;
        [Tooltip("High-score 'Score' Column")] [SerializeField]
        private TextMeshProUGUI scoreColumnTrack;
        
        [Header("Lap Text Fields")]
        
        [Tooltip("High-score 'Name' Column")] [SerializeField]
        private TextMeshProUGUI nameColumnLab;
        [Tooltip("High-score 'Score' Column")] [SerializeField]
        private TextMeshProUGUI scoreColumnLab;

        [Tooltip("Row count | How many high-scores to show at once")] [SerializeField]
        private int maximumEntries = 12;

        private const string SubmitTrackScoreUri = "https://???/api/track_times?user={0}&time={1}";
        private const string SubmitLapScoreUri = "https://???/api/lap_times?user={0}&time={1}";
        private const string GetTrackScoresUri = "https://???/api/track_times";
        private const string GetLapScoresUri = "https://???/api/lap_times";

        private readonly string defaultText = string.Concat(Enumerable.Repeat($"Loading...{Environment.NewLine}", 4));

        // The callback used to update text with leaderboard information that we have retrieved from the server.
        private delegate void Callback(ScoreEntryList entryList, ScoreType scoreType);
        
        private void Start()
        {
            if (nameColumnTrack != default)
            {
                // Set the text of the columns to display "Loading..." on each line.
                nameColumnTrack.text = defaultText;
                scoreColumnTrack.text = defaultText;
                nameColumnLab.text = defaultText;
                scoreColumnLab.text = defaultText;

                // Request global score information from the server, and provide a callback for when we get that information.
                StartCoroutine(GetGlobalScoresRequest(UpdateTextFields, ScoreType.Track));
                StartCoroutine(GetGlobalScoresRequest(UpdateTextFields, ScoreType.Lap));
            }
        }

        public enum ScoreType
        {
            Track,
            Lap
        };
        
        // Post a user's score to the leaderboard server.
        public void SubmitScore(ScoreType scoreType, string user, int score)
        {
            
            if (user == default)
            {
                Debug.Log("Not submitting a score as there is no player name.");
                return;
            }

            if (score == default)
            {
                Debug.Log("Not submitting the score as there is no score.");
                return;
            }

            StartCoroutine(SubmitScoreCoroutine(scoreType, user, score));
        }

        private static IEnumerator SubmitScoreCoroutine(ScoreType scoreType, string user, int score)
        {
            string uri = scoreType == ScoreType.Track ? SubmitTrackScoreUri : SubmitLapScoreUri;
            using UnityWebRequest ping = UnityWebRequest.PostWwwForm(string.Format(uri, user, score), "");
            yield return ping.SendWebRequest();

            switch (ping.result)
            {
                case UnityWebRequest.Result.Success:
                    string data = ping.downloadHandler.text;
                    // Query succeeded. Convert from JSON string to objects, and then execute the callback.
                    Debug.Log(data);
                    break;
                case UnityWebRequest.Result.InProgress:
                    Debug.Log("Query is in progress.");
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log("A connection error occurred.");
                    Debug.Log(ping.responseCode);
                    Debug.Log(ping.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("A protocol error occurred.");
                    Debug.Log(ping.responseCode);
                    Debug.Log(ping.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log("A data processing error occurred.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Get a list of scores from the leaderboard server.
        private static IEnumerator GetGlobalScoresRequest(Callback callback, ScoreType scoreType)
        {
            string uri = scoreType == ScoreType.Track ? GetTrackScoresUri : GetLapScoresUri;
            using UnityWebRequest req = UnityWebRequest.Get(uri);
            yield return req.SendWebRequest();

            switch (req.result)
            {
                case UnityWebRequest.Result.Success:
                    string data = req.downloadHandler.text;
                    // Query succeeded. Convert from JSON string to objects, and then execute the callback.
                    ScoreEntryList entryList = JsonUtility.FromJson<ScoreEntryList>("{\"entries\": " + data + "}");
                    callback.Invoke(entryList, scoreType);
                    break;
                case UnityWebRequest.Result.InProgress:
                    Debug.Log("Query is in progress.");
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log("A connection error occurred.");
                    Debug.Log(req.responseCode);
                    Debug.Log(req.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("A protocol error occurred.");
                    Debug.Log(req.responseCode);
                    Debug.Log(req.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log("A data processing error occurred.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Update the text fields with the information that we have received from the leaderboard server.
        private void UpdateTextFields(ScoreEntryList entryList, ScoreType scoreType)
        {
            StringBuilder nameBuilder = new StringBuilder();
            StringBuilder scoreBuilder = new StringBuilder();

            for (int i = 0; i < maximumEntries && i < entryList.entries.Length; ++i)
            {
                nameBuilder.AppendLine(entryList.entries[i].user);
                scoreBuilder.AppendLine(entryList.entries[i].score.ToString());
            }
            
            if (scoreType == ScoreType.Track)
            {
                nameColumnTrack.text = nameBuilder.ToString();
                scoreColumnTrack.text = scoreBuilder.ToString();
            }
            else
            {
                nameColumnLab.text = nameBuilder.ToString();
                scoreColumnLab.text = scoreBuilder.ToString(); 
            }
        }
    }

// Class objects for JSON deserialization.

    [Serializable]
    public class ScoreEntryList
    {
        public ScoreEntry[] entries;
    }

    [Serializable]
    public class ScoreEntry
    {
        public string user;
        public int score;
    }
}