using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;///���J��ܮw

public class menu1 : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene("NetworkSandbox");
        ///���J�C������
    }

   public void QuitGame()
    {
        Application.Quit();
        ///�����C��
    }

    public void Menu()
    {
        SceneManager.LoadScene("Loby");
        ///�^������
    }

}
