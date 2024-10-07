using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreEntry : MonoBehaviour
{
    public TextMeshProUGUI numberTxt;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI timeTxt;
    
    public void SetNumber(int num)
    {
        numberTxt.text = $"{num.ToString()}.";
    }

    public void SetName(string entryName)
    {
        int stringLimit = entryName.Length - 1 <= 12 ? entryName.Length : 12;
        nameTxt.text = entryName.Substring(0, stringLimit);
    }

    public void SetTime(float time)
    {
        timeTxt.text = StringUtils.ConvertFloatToMinutesSecondsMilliseconds(time, ":");
    }
}
