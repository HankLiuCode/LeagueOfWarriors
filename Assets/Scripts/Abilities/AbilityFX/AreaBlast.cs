using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBlast : MonoBehaviour
{
    [SerializeField] ParticleSystem colorBlast;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem groundCrack;

    [SerializeField] float size = 1f;
    [SerializeField] Color color = Color.red;

    void Start()
    {
        
    }

    void Update()
    {
        ParticleSystem.MainModule colorBlastMain = colorBlast.main;
        colorBlastMain.startSize3D = true;
        colorBlastMain.startSizeX = size;
        colorBlastMain.startSizeZ = size;
        colorBlastMain.startColor = color;

        ParticleSystem.MainModule groundBreakMain = groundCrack.main;
        groundBreakMain.startSize = new ParticleSystem.MinMaxCurve(size / 2);

        ParticleSystem.ShapeModule smokeShape = smoke.shape;
        smokeShape.radius = size / 6;
    }
}
