using Dota.Controls;
using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Minimap : MonoBehaviour
{
    [SerializeField] private VisionChecker visibilityChecker = null;
    [SerializeField] private CameraController cameraController = null;

    [SerializeField] private Vector2 xMinMax = new Vector2(-50, 50);
    [SerializeField] private Vector2 zMinMax = new Vector2(-50, 50);

    [SerializeField] private RectTransform minimapRect = null;
    [SerializeField] private RectTransform cameraRect = null;

    [SerializeField] private RectTransform defaultIconLayer = null;
    [SerializeField] private RectTransform minionIconLayer = null;
    [SerializeField] private RectTransform playerIconLayer = null;
    
    private Vector2 defaultCameraRectSize = new Vector2(80, 45);
    private Dictionary<Transform, MinimapIcon> minimapIconInstances = new Dictionary<Transform, MinimapIcon>();

    private void Awake()
    {
        visibilityChecker.OnVisionEntityAdded += VisibilityChecker_OnVisionEntityAdded;
        visibilityChecker.OnVisionEntityRemoved += VisibilityChecker_OnVisionEntityRemoved;
    }

    private void VisibilityChecker_OnVisionEntityRemoved(VisionEntity visionEntity)
    {
        minimapIconInstances.Remove(visionEntity.transform);
    }

    private void VisibilityChecker_OnVisionEntityAdded(VisionEntity visionEntity)
    {
        IMinimapEntity minimapEntity = visionEntity.GetComponent<IMinimapEntity>();

        MinimapIcon minimapIconInstance = minimapEntity.GetMinimapIconInstance();

        minimapIconInstance.transform.parent = GetLayer(minimapEntity.GetLayerName());

        minimapIconInstances.Add(visionEntity.transform, minimapIconInstance);
    }

    private Transform GetLayer(string layerName)
    {
        switch (layerName)
        {
            case "Champion":
                return playerIconLayer;

            case "Minion":
                return minionIconLayer;

            default:
                return defaultIconLayer;
        }
    }

    private void UpdatePosition(Vector3 worldObjectPos, Transform uiInstance)
    {
        Vector2 normPos = new Vector2(
            (worldObjectPos.x - xMinMax.x) / (xMinMax.y - xMinMax.x),
            (worldObjectPos.z - zMinMax.x) / (zMinMax.y - zMinMax.x)
        );

        Vector2 minimapPos = new Vector2(
                normPos.x * minimapRect.rect.width + minimapRect.rect.x,
                normPos.y * minimapRect.rect.height + minimapRect.rect.y
        );
        uiInstance.localPosition = minimapPos;
    }

    private void UpdatePosition(Transform worldObject, Transform uiInstance)
    {
        UpdatePosition(worldObject.position, uiInstance);
    }

    private void Update()
    {
        foreach (VisionEntity visionEntity in visibilityChecker.GetAll())
        {
            minimapIconInstances[visionEntity.transform].SetVisible(visionEntity.GetVisible());
            UpdatePosition(visionEntity.transform, minimapIconInstances[visionEntity.transform].transform);
        }

        UpdatePosition(cameraController.GetLookAtPoint(), cameraRect.transform);

        float width = defaultCameraRectSize.x * (cameraController.GetViewDist() / CameraController.MAX_VIEW_DISTANCE);
        float height = defaultCameraRectSize.y * (cameraController.GetViewDist() / CameraController.MAX_VIEW_DISTANCE);
        cameraRect.sizeDelta = new Vector2(width, height);

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
