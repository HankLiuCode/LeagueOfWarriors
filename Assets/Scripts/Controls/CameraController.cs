using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float screenBorderThickness = 30f;
    [SerializeField] float speed = 15f;
    [SerializeField] Vector3 offset = new Vector3(0, 0, 9);
    [SerializeField] Transform target = null;
    float defaultHeight = 10;


    private void Start()
    {
        transform.position = new Vector3(transform.position.x, defaultHeight, transform.position.z);
    }


    void Update()
    {
        if (!Application.isFocused) { return; }

        if (Input.GetKey(KeyCode.Space))
        {
            FollowTarget();
        }
        UpdateCameraPosition();
    }

    public void SetFollowTarget(Transform target)
    {
        this.target = target;
    }

    private void FollowTarget()
    {
        Vector3 camPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        camPos += offset;
        transform.position = camPos;
    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = transform.position;

        Vector3 cursorMovement = Vector3.zero;

        Vector2 cursorPosition = Input.mousePosition;

        if (cursorPosition.y >= Screen.height - screenBorderThickness)
        {
            cursorMovement.z += 1;
        }
        else if (cursorPosition.y <= screenBorderThickness)
        {
            cursorMovement.z -= 1;
        }

        if (cursorPosition.x >= Screen.width - screenBorderThickness)
        {
            cursorMovement.x += 1;
        }
        else if (cursorPosition.x <= screenBorderThickness)
        {
            cursorMovement.x -= 1;
        }
        pos += cursorMovement.normalized * speed * Time.deltaTime;

        transform.position = pos;
    }
}
