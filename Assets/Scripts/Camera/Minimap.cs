using Dota.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private RectTransform minimapRect = null;
    [SerializeField] private CameraController cameraController = null;

    [SerializeField] private Transform worldObj = null;

    [SerializeField] private RectTransform testUI = null;

    private void Update()
    {
        if(cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
        }

        if(worldObj == null)
        {
            worldObj = FindObjectOfType<DotaPlayerController>().transform;
        }

        Vector2 xMinMax = cameraController.GetXMinMax();
        Vector2 zMinMax = cameraController.GetZMinMax();

        //Vector2 normPos = new Vector2(
        //    (worldObj.position.x - xMinMax.x) / (xMinMax.y - xMinMax.x),
        //    (worldObj.position.z - zMinMax.x) / (zMinMax.y - zMinMax.x)
        //);

        Vector2 normPos = new Vector2(
            (worldObj.position.x - -50) / 100,
            (worldObj.position.z - -50) / 100
        );

        Vector2 minimapPos = new Vector2(
                normPos.x * minimapRect.rect.width + minimapRect.rect.x,
                normPos.y * minimapRect.rect.height + minimapRect.rect.y
        );
        testUI.localPosition = minimapPos;

        if (Input.GetMouseButton(0))
        {
            MoveCameraToMinimapClickedPosition();
        }
    }

    private void MoveCameraToMinimapClickedPosition()
    {
        Vector3 mousePos = Input.mousePosition;

        bool inMap = RectTransformUtility.RectangleContainsScreenPoint(minimapRect, mousePos);

        if (!inMap) { return; }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, mousePos, null, out Vector2 localPoint);

        Vector2 normMinimapPos = new Vector2(
            (localPoint.x - minimapRect.rect.x) / minimapRect.rect.width,
            (localPoint.y - minimapRect.rect.y) / minimapRect.rect.height
        );

        //Debug.Log(mousePos + "->" + localPoint + "," + normMinimapPos);

        Vector2 xMinMax = cameraController.GetXMinMax();
        Vector2 zMinMax = cameraController.GetZMinMax();

        Vector3 lookAtPos = new Vector3(
            Mathf.Lerp(xMinMax.x, xMinMax.y, normMinimapPos.x),
            0,
            Mathf.Lerp(zMinMax.x, zMinMax.y, normMinimapPos.y)
        );

        cameraController.UpdateCameraPosition(lookAtPos);
    }
}
