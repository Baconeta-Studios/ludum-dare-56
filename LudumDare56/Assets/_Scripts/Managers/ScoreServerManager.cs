using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Utils;

namespace _Scripts.Managers
{
    public class ScoreServerManager : Singleton<ScoreServerManager>
    {
        public bool AllowSubmitScoreInEditor;
        public bool DoYouReallyWantToSubmitInEditor;
        private const string RootUri = "https://ld56-ddss.free.beeceptor.com";
        private const string SubmitTrackScoreUri = RootUri + "/api/track_times?user={0}&score={1}";
        private const string SubmitLapScoreUri = RootUri + "/api/lap_times?user={0}&score={1}";
        private const string GetTrackScoresUri = RootUri + "/api/track_times";
        private const string GetLapScoresUri = RootUri + "/api/lap_times";

        public HighScoreCollection trackData;
        public HighScoreCollection lapData;
        
        public static event Action<HighScoreCollection> OnTrackDataUpdate;
        public static event Action<HighScoreCollection> OnLapDataUpdate;

        public HighScoreCollection GetTrackData(bool fetchDataFromServer=true)
        {
            if (fetchDataFromServer)
            {
                RefetchTrackData();
            }
            return trackData;
        }
        
        public HighScoreCollection GetLapData(bool fetchDataFromServer=true)
        {
            if (fetchDataFromServer)
            {
                RefetchLapData();
            }
            return lapData;
        }
        
        // The callback used to update text with leaderboard information that we have retrieved from the server.
        private delegate void Callback(HighScoreCollection entryList, ScoreType scoreType);

        private void SaveData(HighScoreCollection entryList, ScoreType scoreType)
        {
            if (scoreType == ScoreType.Track)
            {
                trackData = entryList;
                OnTrackDataUpdate?.Invoke(trackData);
            }
            else if (scoreType == ScoreType.Lap)
            {
                lapData = entryList;
                OnLapDataUpdate?.Invoke(lapData);
            }
        }

        [ContextMenu("Refresh Scores")]
        private void RefetchTrackData()
        {
            StartCoroutine(GetGlobalScoresRequest(SaveData, ScoreType.Track));
        }

        private void RefetchLapData()
        {
            StartCoroutine(GetGlobalScoresRequest(SaveData, ScoreType.Lap));
        }

        public enum ScoreType
        {
            Track,
            Lap
        };
        
        // Post a user's score to the leaderboard server.
        public void SubmitScore(ScoreType scoreType, string user, float score)
        {
            
            #if (AllowSubmitScoreInEditor && DoYouReallyWantToSubmitInEditor) || !UNITY_EDITOR
            
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
            
            #endif
        }

        private static IEnumerator SubmitScoreCoroutine(ScoreType scoreType, string user, float score)
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
                    HighScoreCollection entryList = JsonUtility.FromJson<HighScoreCollection>("{\"highScores\": " + data + "}");
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
    }

// Class objects for JSON deserialization.
    [Serializable]
    public struct HighScoreCollection
    {
        public HighScore[] highScores;
    }
    
    [Serializable]
    public struct HighScore
    {
        // user Name
        public string user;
        // Time seconds
        public int score;
    }
}