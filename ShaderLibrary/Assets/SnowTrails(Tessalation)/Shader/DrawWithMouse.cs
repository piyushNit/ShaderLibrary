using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    public Camera _Camera;
    public Shader _DrawShader;

    private RenderTexture _SplatMap;

    private Material _SnowMaterial, _DrawMaterial;

    private RaycastHit rayHit;
    [Range(1.0f, 500.0f)]
    public float brushSize = 200;
    [Range(0, 1)]
    public float brushStrength = 1;

    private void Start()
    {
        _DrawMaterial = new Material(_DrawShader);
        _DrawMaterial.SetVector("_Color", Color.red);

        _SnowMaterial = GetComponent<MeshRenderer>().material;

        _SplatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        _SnowMaterial.SetTexture("_Splat", _SplatMap);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = _Camera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction);

            if(Physics.Raycast(ray, out rayHit))
            {
                _DrawMaterial.SetVector("_Coordinate", new Vector4(rayHit.textureCoord.x, rayHit.textureCoord.y, 0, 0));
                RenderTexture tempTexture = RenderTexture.GetTemporary(_SplatMap.width, _SplatMap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(_SplatMap, tempTexture);
                Graphics.Blit(tempTexture, _SplatMap, _DrawMaterial);
                RenderTexture.ReleaseTemporary(tempTexture);
            }
        }
        _DrawMaterial.SetFloat("_BrushSize", brushSize);
        _DrawMaterial.SetFloat("_BrushStrength", brushStrength);
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), _SplatMap, ScaleMode.ScaleToFit, false, 1);
    }
}
