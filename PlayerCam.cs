using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Camera Settings")]
    public float sensX = 100f;
    public float sensY = 100f;
    public Transform orientation;

    [Header("Candy Pickup Settings")]
    public float pickupDistance = 8f;
    public LayerMask candyLayer;
    public int candyCount = 0;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleCandyPickup();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // pickup candy
    void HandleCandyPickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, candyLayer))
            {
                GameObject candy = hit.collider.gameObject;
                Destroy(candy);
                candyCount++;
                Debug.Log("Candy Picked Up! Total: " + candyCount);
            }
        }
    }
}
