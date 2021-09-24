using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public const float MIN_VIEW_DISTANCE = 10;
    public const float MAX_VIEW_DISTANCE = 20;

    [SerializeField] Camera playerCam = null;

    [SerializeField] Vector2 xMinMax = new Vector2(-45, 45);
    [SerializeField] Vector2 zMinMax = new Vector2(-40, 50);
    [SerializeField] float screenBorderThickness = 30f;
    [SerializeField] float speed = 15f;
    [SerializeField] float viewAngle = 65f;
    [SerializeField] float fov = 30f;

    [Range(MIN_VIEW_DISTANCE, MAX_VIEW_DISTANCE)]
    [SerializeField] 
    float viewDist = MAX_VIEW_DISTANCE;

    Vector3 lookAtPoint;
    Transform followTarget = null;

    public Camera GetCamera()
    {
        return playerCam;
    }

    public Vector3 GetLookAtPoint()
    {
        return lookAtPoint;
    }

    public float GetViewDist()
    {
        return viewDist;
    }

    public Vector2 GetXMinMax()
    {
        return xMinMax;
    }

    public Vector2 GetZMinMax()
    {
        return zMinMax;
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
        lookAtPoint = target.position;
        UpdateCameraPosition(viewAngle, viewDist, target.position);
    }
    
    private void UpdateCameraPosition(float angle, float distance, Vector3 target)
    {
        angle = Mathf.Clamp(angle, 0, 90);

        float angleFromUp = 90 - angle;

        Vector3 targetToCamDir = Quaternion.AngleAxis(-angleFromUp, Vector3.right) * Vector3.up;

        Vector3 camPos = target + targetToCamDir * distance;

        playerCam.transform.rotation = Quaternion.AngleAxis(angle, Vector3.right) * Quaternion.identity;

        playerCam.transform.position = camPos;
    }

    public void UpdateCameraPosition(Vector3 target)
    {
        lookAtPoint = target;
    }

    void Update()
    {
        if (!Application.isFocused) { return; }
        
        if (Input.GetKey(KeyCode.Space))
        {
            if(followTarget == null) { return; }

            lookAtPoint = followTarget.position;

            UpdateCameraPosition(viewAngle, viewDist, lookAtPoint);
        }
        else
        {
            lookAtPoint = lookAtPoint + GetMouseMovement() * speed * Time.deltaTime;

            float lookAtPointX = Mathf.Clamp(lookAtPoint.x, xMinMax.x, xMinMax.y);

            float lookAtPointZ = Mathf.Clamp(lookAtPoint.z, zMinMax.x, zMinMax.y);

            lookAtPoint = new Vector3(lookAtPointX, 0, lookAtPointZ);

            UpdateCameraPosition(viewAngle, viewDist, lookAtPoint);
        }

        playerCam.fieldOfView = fov;
        viewDist = Mathf.Clamp(viewDist - Input.mouseScrollDelta.y, MIN_VIEW_DISTANCE, MAX_VIEW_DISTANCE);
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

    public static Ray GetMouseRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
