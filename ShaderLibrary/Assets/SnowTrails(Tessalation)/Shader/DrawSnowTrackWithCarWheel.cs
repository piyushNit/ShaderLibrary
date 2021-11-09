using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSnowTrackWithCarWheel : MonoBehaviour
{
    public Shader _DrawShader;
    public GameObject _Ground;

    private RenderTexture _SplatMap;

    private Material _SnowMaterial, _DrawMaterial;

    [Range(1.0f, 500.0f)]
    public float brushSize = 200;
    [Range(0, 1)]
    public float brushStrength = 1;

    public Transform[] _Wheels;
    public LayerMask groundMask;
    private RaycastHit rayHit;

    private void Start()
    {
        _DrawMaterial = new Material(_DrawShader);
        _DrawMaterial.SetVector("_Color", Color.red);

        _SnowMaterial = _Ground.GetComponent<MeshRenderer>().material;

        _SplatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        _SnowMaterial.SetTexture("_Splat", _SplatMap);
    }

    private void Update()
    {
        _DrawMaterial.SetFloat("_BrushSize", brushSize);
        _DrawMaterial.SetFloat("_BrushStrength", brushStrength);
        for (int i = 0; i < _Wheels.Length; i++)
        {
            Ray ray = new Ray(_Wheels[i].position, Vector3.down);
            Debug.DrawRay(ray.origin, ray.direction);
            if(Physics.Raycast(ray, out rayHit, 1, groundMask))
            {
                _DrawMaterial.SetVector("_Coordinate", new Vector4(rayHit.textureCoord.x, rayHit.textureCoord.y, 0, 0));
                RenderTexture tempTexture = RenderTexture.GetTemporary(_SplatMap.width, _SplatMap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(_SplatMap, tempTexture);
                Graphics.Blit(tempTexture, _SplatMap, _DrawMaterial);
                RenderTexture.ReleaseTemporary(tempTexture);
            }
        }
    }
}
