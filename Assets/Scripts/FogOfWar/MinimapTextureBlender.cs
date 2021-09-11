using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapTextureBlender : MonoBehaviour
{
    [SerializeField] RenderTexture preShroudRTex = null;
    [SerializeField] Texture2D minimapTex = null;

    Texture2D preShroudTexture;

    [SerializeField] RenderTexture shroudRTex = null;
    
    private void Update()
    {
        preShroudTexture = RenderTexToTexture2D(preShroudRTex);

        Color[] colors = preShroudTexture.GetPixels();
        Color[] colors2 = minimapTex.GetPixels(4);

        for(int i=0; i<colors.Length; i++)
        {
            colors[i] = colors[i] * colors2[i];
        }
        preShroudTexture.SetPixels(colors);
        preShroudTexture.Apply();

        Graphics.Blit(preShroudTexture, shroudRTex);
    }


    Texture2D RenderTexToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
