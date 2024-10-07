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
        foreach (var trafficLight in trafficLights)
        { 
            if(!trafficLight.image.enabled) trafficLight.image.enabled = true;
            
            trafficLight.image.sprite = trafficLight.sprites[timeRemaining];
        }
        
        if(timeRemaining == 0)
        {                    
            animator.SetTrigger("Hide");
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
