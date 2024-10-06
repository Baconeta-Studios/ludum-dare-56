using System;
using UnityEngine;

public class CardTrayUIManager : MonoBehaviour
{
    public Vector3 trayUIStartPosition;
    public Vector3 trayUIOpenPosition;
    
    private bool isTrayUIOpen = false;
    
    public event Action OnTrayUIOpen;
    public event Action OnTrayUIClose;
    
    // Get Player Deck system controller
    
    private void Start()
    {
        gameObject.transform.localPosition = trayUIStartPosition;
    }

    public void ToggleTrayUI()
    {
        if (isTrayUIOpen)
        {
            CloseTrayUI();
        }
        else
        {
            OpenTrayUI();
        }
    }

    private void OpenTrayUI()
    {
        Debug.Log("Opening Tray UI");
        if (isTrayUIOpen)
        {
            // Do nothing
            return;
        }

        gameObject.transform.localPosition = trayUIOpenPosition;
        isTrayUIOpen = true;
        OnTrayUIOpen?.Invoke();

        Time.timeScale = 0.2f;
    }

    private void CloseTrayUI()
    {
        Debug.Log("Closing Tray UI");
        if (!isTrayUIOpen)
        {
            // Do nothing
            return;
        }

        gameObject.transform.localPosition = trayUIStartPosition;
        isTrayUIOpen = false;
        OnTrayUIClose?.Invoke();
        
        Time.timeScale = 1f;
    }
}
