using Dota.Controls;
using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Minimap : MonoBehaviour
{
    [SerializeField] private VisibilityChecker visibilityChecker = null;
    [SerializeField] private CameraController cameraController = null;

    [SerializeField] private Vector2 xMinMax = new Vector2(-50, 50);
    [SerializeField] private Vector2 zMinMax = new Vector2(-50, 50);

    [SerializeField] private RectTransform minimapRect = null;
    [SerializeField] private RectTransform cameraRect = null;

    [SerializeField] private RectTransform minionIconLayer = null;
    [SerializeField] private RectTransform playerIconLayer = null;

    [SerializeField] private MinimapIcon minimapMinionIconPrefab = null;
    [SerializeField] private MinimapPlayerIcon minimapPlayerIconPrefab = null;


    private Vector2 defaultCameraRectSize = new Vector2(80, 45);
    private Dictionary<Transform, MinimapIcon> minimapIconInstances = new Dictionary<Transform, MinimapIcon>();

    private void Start()
    {
        visibilityChecker.OnVisionEntityAdded += VisibilityChecker_OnVisionEntityAdded;
        visibilityChecker.OnVisionEntityRemoved += VisibilityChecker_OnVisionEntityRemoved;
        visibilityChecker.OnAllPlayersAdded += VisibilityChecker_OnAllPlayersAdded;
    }

    private void VisibilityChecker_OnAllPlayersAdded()
    {
        foreach (VisionEntity enemy in visibilityChecker.GetEnemies())
        {
            MinimapPlayerIcon minimapIconInstance = Instantiate(minimapPlayerIconPrefab, playerIconLayer.transform);
            minimapIconInstance.SetTeam(enemy.GetComponent<DotaGamePlayer>().GetTeam());
            minimapIconInstance.SetPlayerIcon(enemy.GetComponent<DotaGamePlayer>().GetPlayerSprite());
            minimapIconInstances.Add(enemy.GetComponent<DotaGamePlayer>().transform, minimapIconInstance);
        }

        foreach (VisionEntity ally in visibilityChecker.GetAllies())
        {
            MinimapPlayerIcon minimapIconInstance = Instantiate(minimapPlayerIconPrefab, playerIconLayer.transform);
            minimapIconInstance.SetTeam(ally.GetComponent<DotaGamePlayer>().GetTeam());
            minimapIconInstance.SetPlayerIcon(ally.GetComponent<DotaGamePlayer>().GetPlayerSprite());
            minimapIconInstances.Add(ally.GetComponent<DotaGamePlayer>().transform, minimapIconInstance);
        }
    }

    private void VisibilityChecker_OnVisionEntityRemoved(VisionEntity obj)
    {
        minimapIconInstances.Remove(obj.transform);
    }

    private void VisibilityChecker_OnVisionEntityAdded(VisionEntity obj)
    {
        Minion minion = obj.GetComponent<Minion>();
        MinimapIcon minimapIconInstance = Instantiate(minimapMinionIconPrefab, minionIconLayer.transform).GetComponent<MinimapIcon>();
        minimapIconInstance.SetTeam(minion.GetTeam());
        minimapIconInstances.Add(obj.transform, minimapIconInstance);
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
        foreach (VisionEntity enemy in visibilityChecker.GetEnemies())
        {
            minimapIconInstances[enemy.transform].SetVisible(enemy.GetVisible());
            UpdatePosition(enemy.transform, minimapIconInstances[enemy.transform].transform);
        }

        foreach (VisionEntity ally in visibilityChecker.GetAllies())
        {
            minimapIconInstances[ally.transform].SetVisible(true);
            UpdatePosition(ally.transform, minimapIconInstances[ally.transform].transform);
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
