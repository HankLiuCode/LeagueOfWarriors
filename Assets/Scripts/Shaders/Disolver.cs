using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disolver : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] Shader disolveShader = null;
    [SerializeField] float disolveDuration = 1f; 


    public float GetDisolveDuration()
    {
        return disolveDuration;
    }

    public void StartDisolve()
    {
        if(disolveShader != null)
        {
            meshRenderer.material.shader = disolveShader;
        }
        StartCoroutine(DisolveRoutine());
    }

    IEnumerator DisolveRoutine()
    {
        float value = 0;
        while(value <= 1)
        {
            value += (1 / disolveDuration) * Time.deltaTime;
            meshRenderer.material.SetFloat("_DissolveValue", value);
            yield return null;
        }
    }
}
