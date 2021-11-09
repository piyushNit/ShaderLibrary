using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFall : MonoBehaviour
{
    public Shader snowFallShader;
    private Material snowfallMaterial;
    private MeshRenderer meshRenderer;

    [Range(0.001f, 0.1f)]
    public float flakeAmount;
    [Range(0, 1)]
    public float flakeOpacity;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        snowfallMaterial = new Material(snowFallShader);
    }

    void Update()
    {
        snowfallMaterial.SetFloat("_FlakeAmount", flakeAmount);
        snowfallMaterial.SetFloat("_FlakeOpacity", flakeOpacity);

        RenderTexture snow = (RenderTexture)meshRenderer.material.GetTexture("_Splat");
        RenderTexture temp = RenderTexture.GetTemporary(snow.width, snow.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(snow, temp, snowfallMaterial);
        Graphics.Blit(temp, snow);
        meshRenderer.material.SetTexture("_Splat", snow);
        RenderTexture.ReleaseTemporary(temp);
    }
}
