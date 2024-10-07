using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostRaceUi : MonoBehaviour
{
    public GameObject uiContainer;
    
    void OnEnable()
    {
        RaceManager.OnRaceCompleted += ShowUi;
    }

    void OnDisable()
    {
        RaceManager.OnRaceCompleted -= ShowUi;
    }

    void ShowUi()
    {
        uiContainer.SetActive(true);
    }
}
