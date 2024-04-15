using UnityEngine;

public class PlayerNavigation : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public float dragSpeed = 2f; // Drag speed for changing facing direction
    private Vector3 dragOrigin; // Origin position for drag movement

    void Update()
    {
        // Handle drag input to change facing direction
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        transform.RotateAround(transform.position, transform.right, -pos.y * dragSpeed);
        transform.RotateAround(transform.position, Vector3.up, pos.x * dragSpeed);

        // Clamp rotation to prevent player from flipping upside down
        float xRotation = transform.rotation.eulerAngles.x;
        if (xRotation > 180f)
        {
            xRotation -= 360f;
        }
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, 0f);
    }

    void FixedUpdate()
    {
        // Handle movement input using WASD keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        moveDirection = transform.TransformDirection(moveDirection); // Convert to world space

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
