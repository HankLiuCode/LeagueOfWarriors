using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;///載入函示庫

public class menu1 : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene("GameView");
        ///載入遊戲場景
    }

   public void QuitGame()
    {
        Application.Quit();
        ///結束遊戲
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
        ///回選單場景
    }

}
