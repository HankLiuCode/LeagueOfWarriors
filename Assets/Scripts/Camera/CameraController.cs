using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public const float MIN_VIEW_DISTANCE = 5;
    public const float MAX_VIEW_DISTANCE = 30;

    [SerializeField] Camera playerCam = null;
    [SerializeField] float screenBorderThickness = 30f;
    [SerializeField] float speed = 15f;
    [SerializeField] float viewAngle = 65f;
    [SerializeField] float fov = 30f;

    [Range(MIN_VIEW_DISTANCE, MAX_VIEW_DISTANCE)]
    [SerializeField] 
    float viewDist = MAX_VIEW_DISTANCE;

    Vector3 lookAtPoint;
    Transform followTarget = null;

    public void Initialize(Transform target)
    {
        followTarget = target;
        UpdateCameraPosition(viewAngle, viewDist, target.position);
    }
    
    /// <summary>
    /// Updates the playerCam position & rotation given the angle, distance and target
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="distance"></param>
    /// <param name="target"></param>
    /// 
    public void UpdateCameraPosition(float angle, float distance, Vector3 target)
    {
        angle = Mathf.Clamp(angle, 0, 90);

        float angleFromUp = 90 - angle;

        Vector3 targetToCamDir = Quaternion.AngleAxis(-angleFromUp, Vector3.right) * Vector3.up;

        Vector3 camPos = target + targetToCamDir * distance;

        playerCam.transform.rotation = Quaternion.AngleAxis(angle, Vector3.right) * Quaternion.identity;

        playerCam.transform.position = camPos;
    }


    void Update()
    {
        if (!Application.isFocused) { return; }

        if (Input.GetKey(KeyCode.Space))
        {
            lookAtPoint = followTarget.position;
            UpdateCameraPosition(viewAngle, viewDist, lookAtPoint);
        }
        else
        {
            lookAtPoint += GetMouseMovement() * speed * Time.deltaTime;
            UpdateCameraPosition(viewAngle, viewDist, lookAtPoint);
        }

        playerCam.fieldOfView = fov;
        viewDist = Mathf.Clamp(viewDist- Input.mouseScrollDelta.y, MIN_VIEW_DISTANCE, MAX_VIEW_DISTANCE);
    }

    private Vector3 GetMouseMovement()
    {
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

        return cursorMovement.normalized;
    }
}
