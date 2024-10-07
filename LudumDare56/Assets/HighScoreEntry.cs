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
        nameTxt.text = entryName.ToString();
    }

    public void SetTime(float time)
    {
        timeTxt.text = time.ToString();
    }
}
