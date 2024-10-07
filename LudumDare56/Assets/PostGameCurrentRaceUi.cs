using System.Collections;
using System.Collections.Generic;
using _Scripts.Managers;
using TMPro;
using UnityEngine;

public class PostGameCurrentRaceUi : MonoBehaviour
{
    public TextMeshProUGUI playerNameTmp;
    public TextMeshProUGUI placingTmp;
    public TextMeshProUGUI raceTimeTmp;
    public TextMeshProUGUI bestLapTmp;

    public float placingSuffixSize = 120f;

    public void OnEnable()
    {
        // fetch details

        UpdateDetails("Rustle",
            TrackPositionManager.Instance.playerPlacing,
            GameTimeManager.Instance.totalRaceTimeSoFar,
            GameTimeManager.Instance.fastestLap);
    }
    
    private void UpdateDetails(string playerName, int placing, float raceTime, float bestLap)
    {
        playerNameTmp.text = playerName;
        placingTmp.text = StringUtils.FormatPlacing(placing, placingSuffixSize);
        raceTimeTmp.text = StringUtils.ConvertFloatToMinutesSecondsMilliseconds(raceTime, ":");
        bestLapTmp.text = StringUtils.ConvertFloatToMinutesSecondsMilliseconds(bestLap, ":");
    }
}
