using Dota.Controls;
using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Change back to MonoBehaviour When finish testing
public class Minimap : MonoBehaviour
{
    [SerializeField] private VisibilityChecker visibilityChecker = null;
    [SerializeField] private RectTransform minimapRect = null;
    [SerializeField] private CameraController cameraController = null;

    [SerializeField] private MinimapIcon UIPrefab = null;
    private Dictionary<Transform, MinimapIcon> UIInstances = new Dictionary<Transform, MinimapIcon>();

    private void Start()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllPlayersAdded += Minimap_OnAllPlayersAdded;
    }

    private void Minimap_OnAllPlayersAdded()
    {
        List<DotaGamePlayer> dotaGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetDotaGamePlayers();
        foreach(DotaGamePlayer gamePlayer in dotaGamePlayers)
        {
            MinimapIcon uiInstance = Instantiate(UIPrefab, transform);
            UIInstances.Add(gamePlayer.transform, uiInstance);
        }
    }

    private void UpdateIcon(Transform worldObject, Transform uiInstance)
    {
        Vector2 xMinMax = cameraController.GetXMinMax();
        Vector2 zMinMax = cameraController.GetZMinMax();

        //Vector2 normPos = new Vector2(
        //    (worldObject.position.x - xMinMax.x) / (xMinMax.y - xMinMax.x),
        //    (worldObject.position.z - zMinMax.x) / (zMinMax.y - zMinMax.x)
        //);

        Vector2 normPos = new Vector2(
            (worldObject.position.x - (-50)) / 100,
            (worldObject.position.z - (-50)) / 100
        );

        Vector2 minimapPos = new Vector2(
                normPos.x * minimapRect.rect.width + minimapRect.rect.x,
                normPos.y * minimapRect.rect.height + minimapRect.rect.y
        );
        uiInstance.localPosition = minimapPos;
    }

    private void Update()
    {
        List<Transform> worldPositions = new List<Transform>(UIInstances.Keys);

        foreach(Transform worldPos in worldPositions)
        {
            UIInstances[worldPos].SetVisible(false);
        }

        foreach(GameObject visibleEnemy in visibilityChecker.GetVisibleEnemies())
        {
            UIInstances[visibleEnemy.transform].SetVisible(true);
            UpdateIcon(visibleEnemy.transform, UIInstances[visibleEnemy.transform].transform);
        }

        foreach(GameObject ally in visibilityChecker.GetAllies())
        {
            UIInstances[ally.transform].SetVisible(true);
            UpdateIcon(ally.transform, UIInstances[ally.transform].transform);
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
