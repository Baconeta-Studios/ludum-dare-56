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

        var gtm = GameTimeManager.Instance;
        UpdateDetails("Rustle",
            TrackPositionManager.Instance.playerPlacing,
            GameTimeManager.SecondsToCentiseconds(gtm.totalRaceTimeSoFar),
            GameTimeManager.SecondsToCentiseconds(gtm.fastestLap));
    }
    
    private void UpdateDetails(string playerName, int placing, int raceTime, int bestLap)
    {
        playerNameTmp.text = playerName;
        placingTmp.text = StringUtils.FormatPlacing(placing, placingSuffixSize);
        raceTimeTmp.text = StringUtils.ConvertFloatToMinutesSecondsMilliseconds(raceTime, ":");
        bestLapTmp.text = StringUtils.ConvertFloatToMinutesSecondsMilliseconds(bestLap, ":");
    }
}
