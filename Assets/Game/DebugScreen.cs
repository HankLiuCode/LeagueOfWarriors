using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DebugScreen : MonoBehaviour
{
    [SerializeField] NetworkManagerHUD hud = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            hud.showGUI = !hud.showGUI;
        }
    }
}
