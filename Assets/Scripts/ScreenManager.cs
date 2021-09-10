using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(960, 540, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Screen.SetResolution(960, 540, false);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Screen.SetResolution(640, 360, false);
        }
    }
}
