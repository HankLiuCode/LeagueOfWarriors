using Mirror;
using UnityEngine;

public class GameOverCanvas : NetworkBehaviour
{
    [SerializeField] GameObject victory;
    [SerializeField] GameObject defeat;
    [SerializeField] GameObject continueButton;
    [Scene] [SerializeField] string roomScene;

    public void ShowContinueButton()
    {
        continueButton.gameObject.SetActive(true);
    }

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
        if (isServer)
        {
            ((DotaNetworkManager)NetworkManager.singleton).ChangeToRoomScene();
        }
        else
        {
            NetworkClient.Send(new ClientGameToRoomRequestMessage());
        }
    }
}
