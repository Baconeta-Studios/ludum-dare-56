using _Scripts;
using UnityEngine;

public class DraggableCard : MonoBehaviour
{
    public Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private GameInput gameInputSystem;
    private Canvas gameCanvas;
    private Rect rect;

    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
        gameInputSystem = FindFirstObjectByType<GameInput>();
        gameCanvas = FindFirstObjectByType<Canvas>();
        rect = gameCanvas.GetComponent<RectTransform>().rect;
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

    public void OnMouseDown()
    {
        Debug.Log("Mouse Down");
        // When the mouse is clicked, start dragging
        isDragging = true;

        // Calculate offset between object position and mouse position
        offset = transform.position - GetMouseWorldPosition();
    }

    public void OnMouseUp()
    {
        // When the mouse is released, stop dragging
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePoint = gameInputSystem.GetPrimaryPositionScreen;
        if (gameCanvas == null)
        {
            Debug.LogError("No canvas found");
            return new Vector3();
        }

        var mousePosInViewPointCoords = mainCamera.ScreenToViewportPoint(mousePoint);
        mousePosInViewPointCoords.x *= rect.width * gameCanvas.scaleFactor;
        mousePosInViewPointCoords.y *= rect.height * gameCanvas.scaleFactor;

        var mouseScreenPointWithDepth = new Vector3(mousePosInViewPointCoords.x, mousePosInViewPointCoords.y,
            Mathf.Abs(mainCamera.transform.position.z - transform.position.z));

        return mouseScreenPointWithDepth;
    }
}