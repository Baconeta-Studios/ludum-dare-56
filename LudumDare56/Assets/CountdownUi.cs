using UnityEngine;
using UnityEngine.UI;

public class CountdownUi : MonoBehaviour
{
    [SerializeField] private CountdownLightUi[] trafficLights;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        RaceManager.OnRaceCountdownStarting += ShowCountdown;
        RaceManager.OnRaceCountdownChanged += UpdateLights;
    }
    
    private void OnDisable()
    {
        RaceManager.OnRaceCountdownStarting -= ShowCountdown;
        RaceManager.OnRaceCountdownChanged -= UpdateLights;
    }

    private void UpdateLights(int timeRemaining)
    {
        switch (timeRemaining)
        {
            case 0:
                foreach (var trafficLight in trafficLights)
                {
                    foreach (var light in trafficLight.GreenLights)
                    {
                        light.enabled = true;
                    }
                    
                    foreach (var light in trafficLight.RedLights)
                    {
                        light.enabled = false;
                    }
                    animator.SetTrigger("Hide");
                }
                break;
            default:
                foreach (var trafficLight in trafficLights)
                {
                    trafficLight.RedLights[timeRemaining - 1].enabled = true;
                }
                break;
        }
    }

    private void ShowCountdown()
    {
        foreach (var trafficLight in trafficLights)
        {
            trafficLight.gameObject.SetActive(true);
        }
    }

    public void HideCountdown()
    {
        foreach (var trafficLight in trafficLights)
        {
            trafficLight.gameObject.SetActive(false);
        }
    }
}
