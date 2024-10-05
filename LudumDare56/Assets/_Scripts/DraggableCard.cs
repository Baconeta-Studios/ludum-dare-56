using UnityEngine;

public class DraggableCard : MonoBehaviour
{
    public Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;

    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
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
        //Vector3 mousePoint = FindObjectsByType<GameInput>().

        // Set z-coordinate to match the object's z-position
        //mousePoint.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        //return mainCamera.ScreenToWorldPoint(mousePoint);

        return new Vector3(0, 0, 0);
    }
}