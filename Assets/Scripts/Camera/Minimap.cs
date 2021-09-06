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
    [SerializeField] private RectTransform iconParent = null;

    [SerializeField] private MinimapIcon minimapIconPrefab = null;
    private Dictionary<Transform, MinimapIcon> minimapIconInstances = new Dictionary<Transform, MinimapIcon>();

    private void Start()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllGamePlayersAdded += Minimap_OnAllPlayersAdded;
    }

    private void Minimap_OnAllPlayersAdded()
    {
        List<DotaGamePlayer> dotaGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetDotaGamePlayers();
        foreach(DotaGamePlayer gamePlayer in dotaGamePlayers)
        {
            MinimapIcon minimapIconInstance = Instantiate(minimapIconPrefab, iconParent.transform);
            minimapIconInstance.SetTeam(gamePlayer.GetTeam());
            minimapIconInstance.SetPlayerIcon(gamePlayer.GetPlayerSprite());
            minimapIconInstances.Add(gamePlayer.transform, minimapIconInstance);
        }
    }

    private void UpdateIcon(Transform worldObject, Transform uiInstance)
    {
        Vector2 normPos = new Vector2(
            (worldObject.position.x - xMinMax.x) / (xMinMax.y - xMinMax.x),
            (worldObject.position.z - zMinMax.x) / (zMinMax.y - zMinMax.x)
        );

        Vector2 minimapPos = new Vector2(
                normPos.x * minimapRect.rect.width + minimapRect.rect.x,
                normPos.y * minimapRect.rect.height + minimapRect.rect.y
        );
        uiInstance.localPosition = minimapPos;
    }

    private void Update()
    {
        List<Transform> worldPositions = new List<Transform>(minimapIconInstances.Keys);

        foreach (Transform worldPos in worldPositions)
        {
            minimapIconInstances[worldPos].SetVisible(false);
        }

        foreach(VisionEntity enemy in visibilityChecker.GetEnemies())
        {
            minimapIconInstances[enemy.transform].SetVisible(enemy.GetVisible());
            UpdateIcon(enemy.transform, minimapIconInstances[enemy.transform].transform);
        }

        foreach (VisionEntity ally in visibilityChecker.GetAllies())
        {
            minimapIconInstances[ally.transform].SetVisible(true);
            UpdateIcon(ally.transform, minimapIconInstances[ally.transform].transform);
        }

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
