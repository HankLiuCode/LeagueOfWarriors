using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disolver : MonoBehaviour
{
    [SerializeField] Renderer meshRenderer;
    [SerializeField] Shader disolveShader = null;
    [SerializeField] Texture2D disolveTexture = null;
    [SerializeField] float disolveDuration = 1f; 


    public float GetDisolveDuration()
    {
        return disolveDuration;
    }

    public void StartDisolve()
    {
        if(disolveShader != null)
        {
            foreach (Material material in meshRenderer.materials)
            {
                material.shader = disolveShader;
                material.SetTexture("_DissolveTex", disolveTexture);
            }
        }
        StartCoroutine(DisolveRoutine());
    }

    IEnumerator DisolveRoutine()
    {
        float value = 0;
        while(value <= 1)
        {
            value += (1 / disolveDuration) * Time.deltaTime;
            foreach(Material material in meshRenderer.materials)
            {
                material.SetFloat("_DissolveValue", value);
            }
            yield return null;
        }
    }
}
