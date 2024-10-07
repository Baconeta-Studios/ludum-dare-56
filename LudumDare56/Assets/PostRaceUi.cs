using UnityEngine;

public class PostRaceUi : MonoBehaviour
{
    public GameObject postRaceUiContainer;
    public GameObject gameplayUIContainer;
    public GameObject raceInfoUIContainer;

    public void OnEnable()
    {
        RaceManager.OnRaceCompleted += ShowUi;
    }

    private void OnDisable()
    {
        RaceManager.OnRaceCompleted -= ShowUi;
    }

    private void ShowUi()
    {
        postRaceUiContainer.SetActive(true);
        gameplayUIContainer.SetActive(false);
        raceInfoUIContainer.SetActive(false);
    }
}
