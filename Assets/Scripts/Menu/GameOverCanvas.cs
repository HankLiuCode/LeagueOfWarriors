using Mirror;
using UnityEngine;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField] GameObject victory;
    [SerializeField] GameObject defeat;

    public void ShowVictory()
    {
        if(defeat == null || victory == null)
        {
            Debug.LogError("defeat or victory panel is null!");
            return;
        }
        defeat.SetActive(false);
        victory.SetActive(true);
    }

    public void ShowDefeat()
    {
        if (defeat == null || victory == null)
        {
            Debug.LogError("defeat or victory panel is null!");
            return;
        }
        victory.SetActive(false);
        defeat.SetActive(true);
    }

    public void BackToRoom()
    {
        ((DotaNetworkManager) NetworkManager.singleton).ChangeToRoomScene();
    }
}
