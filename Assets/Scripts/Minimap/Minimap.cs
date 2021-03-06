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
    [SerializeField] private RectTransform towerIconLayer = null;
    [SerializeField] private RectTransform playerIconLayer = null;
    
    private Vector2 defaultCameraRectSize = new Vector2(80, 45);
    private Dictionary<VisionEntity, MinimapIcon> minimapIconInstances = new Dictionary<VisionEntity, MinimapIcon>();

    private void Awake()
    {
        visibilityChecker.OnVisionEntityAdded += VisibilityChecker_OnVisionEntityAdded;
        visibilityChecker.OnVisionEntityRemoved += VisibilityChecker_OnVisionEntityRemoved;
    }

    private void VisibilityChecker_OnVisionEntityRemoved(VisionEntity visionEntity)
    {
        if (minimapIconInstances.ContainsKey(visionEntity))
        {
            MinimapIcon icon = minimapIconInstances[visionEntity];
            
            minimapIconInstances.Remove(visionEntity);

            Destroy(icon.gameObject);
        }
        else
        {
            Debug.Log(visionEntity.name + "Doesn't Exist, Remove Failed");
        }
    }

    private void VisibilityChecker_OnVisionEntityAdded(VisionEntity visionEntity)
    {
        if (!minimapIconInstances.ContainsKey(visionEntity))
        {
            IMinimapEntity minimapEntity = visionEntity.GetComponent<IMinimapEntity>();
            MinimapIcon minimapIconInstance = minimapEntity.GetMinimapIconInstance();
            minimapIconInstance.transform.SetParent(GetLayer(minimapEntity.GetLayerName()));
            minimapIconInstances.Add(visionEntity, minimapIconInstance);
        }
        else
        {
            Debug.Log(visionEntity.name + "Already Exist in miniMapIconInstances");
        }
    }

    private Transform GetLayer(string layerName)
    {
        switch (layerName)
        {
            case "Champion":
                return playerIconLayer;

            case "Minion":
                return minionIconLayer;

            case "Building":
                return towerIconLayer;

            case "Monster":
                return defaultIconLayer;

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
        foreach (var visionEntity in minimapIconInstances.Keys)
        {
            minimapIconInstances[visionEntity].SetVisible(visionEntity.GetVisible());

            Vector3 entityPosition = visionEntity.transform.position;

            UpdatePosition(entityPosition, minimapIconInstances[visionEntity].transform);
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
