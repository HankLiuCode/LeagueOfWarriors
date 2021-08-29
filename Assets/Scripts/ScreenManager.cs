using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        //Screen.SetResolution(1280, 1024, false);
    }
}
