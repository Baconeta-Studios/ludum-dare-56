using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    public TMP_InputField inputField;

    public void OnEnable()
    {
        inputField.text = PlayerPrefs.GetString("PlayerName");
        inputField.Select();
    }

    public void SetName()
    {
        string name = inputField.text.Trim();

        PlayerPrefs.SetString("PlayerName", name);
    }
}
