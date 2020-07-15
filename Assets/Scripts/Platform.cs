using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private const float RECYCLE_DISTANCE = 200f;
    [SerializeField] private MeshRenderer topMeshRenderer;
    [SerializeField] private MeshRenderer  bottomMeshRenderer;


    public void SetMaterials(Material topMaterial, Material bottomMaterial =null)
    {
        topMeshRenderer.material = topMaterial;

        if(bottomMaterial != null)
        {
            bottomMeshRenderer.material = bottomMaterial;
        }
    }
    private void Update()
    {
        if(transform.position.z < Player.Instance.transform.position.z - RECYCLE_DISTANCE)
        {
            PlatformManager.Instance?.RecyclePlatform(this);
        }
    }
}
