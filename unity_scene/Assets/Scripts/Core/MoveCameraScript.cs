using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraScript : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of the camera movement
    public float rotateSpeed = 50f; // Speed of the camera rotation
    public float scrollSpeed = 5f; // Speed of the camera movement via scroll

    private Vector3 lastMousePosition;

    // Update is called once per frame
    void Update()
    {
        // Check for mouse scroll to move the camera forward and backward
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        MoveCamera(transform.forward * scroll * scrollSpeed);

        // Check for mouse drag to rotate the camera
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
            RotateCamera(deltaMousePosition);
            lastMousePosition = Input.mousePosition;
        }
    }

    private void MoveCamera(Vector3 direction)
    {
        // Move the camera in the specified direction at the defined speed
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void RotateCamera(Vector3 deltaMousePosition)
    {
        // Rotate the camera based on mouse movement
        float rotateX = deltaMousePosition.y * rotateSpeed * Time.deltaTime;
        float rotateY = deltaMousePosition.x * rotateSpeed * Time.deltaTime;

        transform.Rotate(Vector3.right, -rotateX);
        transform.Rotate(Vector3.up, rotateY, Space.World);
    }
}
