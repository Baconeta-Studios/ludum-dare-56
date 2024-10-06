using UnityEngine;

public class CardTrayUIManager : MonoBehaviour
{
    public Vector3 trayUIStartPosition;
    public Vector3 trayUIOpenPosition;
    
    private bool isTrayUIOpen = false;
    
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

    public void OpenTrayUI()
    {
        Debug.Log("Opening Tray UI");
        if (isTrayUIOpen)
        {
            // Do nothing
            return;
        }

        gameObject.transform.localPosition = trayUIOpenPosition;
        isTrayUIOpen = true;
    }
    
    public void CloseTrayUI()
    {
        Debug.Log("Closing Tray UI");
        if (!isTrayUIOpen)
        {
            // Do nothing
            return;
        }

        gameObject.transform.localPosition = trayUIStartPosition;
        isTrayUIOpen = false;
    }
}
