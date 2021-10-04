using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject audioCanvas;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            bool isActive = audioCanvas.gameObject.activeInHierarchy;
            audioCanvas.gameObject.SetActive(!isActive);
        }
    }
}
