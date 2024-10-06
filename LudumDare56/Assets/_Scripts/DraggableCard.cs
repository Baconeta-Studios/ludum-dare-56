using UnityEngine;

public class DraggableCard : MonoBehaviour
{
    public Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private GameInput gameInputSystem;
    private Canvas cardCanvas;
    private Rect canvasRect;
    private bool isInHandTrigger;
    private CardTrayUIManager cardTrayUIManager;
    private int handSiblingNumber;

    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
        gameInputSystem = FindFirstObjectByType<GameInput>();
        cardCanvas = FindFirstObjectByType<Canvas>();
        canvasRect = cardCanvas.GetComponent<RectTransform>().rect;
        cardTrayUIManager = FindFirstObjectByType<CardTrayUIManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDragging)
        {
            // While dragging, update the position of the object to follow the mouse
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    public void MouseDown()
    {
        // When the mouse is clicked, start dragging
        isDragging = true;
        isInHandTrigger = true;
        handSiblingNumber = transform.GetSiblingIndex();
        gameObject.transform.SetParent(cardCanvas.gameObject.transform, true);

        // Calculate offset between object position and mouse position
        offset = transform.position - GetMouseWorldPosition();
    }

    public void MouseUp()
    {
        // When the mouse is released, stop dragging
        isDragging = false;

        if (isInHandTrigger)
        {
            // Put the card back into the hand
            gameObject.transform.SetParent(cardTrayUIManager.transform, false);
            transform.SetSiblingIndex(handSiblingNumber);
        }
        else // Now we play the card
        {
            // TODO play the card
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePoint = gameInputSystem.GetPrimaryPositionScreen;
        if (cardCanvas == null)
        {
            Debug.LogError("No canvas found");
            return new Vector3();
        }

        var mousePosInViewPointCoords = mainCamera.ScreenToViewportPoint(mousePoint);
        mousePosInViewPointCoords.x *= canvasRect.width * cardCanvas.scaleFactor;
        mousePosInViewPointCoords.y *= canvasRect.height * cardCanvas.scaleFactor;

        var mouseScreenPointWithDepth = new Vector3(mousePosInViewPointCoords.x, mousePosInViewPointCoords.y,
            Mathf.Abs(mainCamera.transform.position.z - transform.position.z));

        return mouseScreenPointWithDepth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hand"))
        {
            isInHandTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Hand"))
        {
            isInHandTrigger = false;
        }
    }
}