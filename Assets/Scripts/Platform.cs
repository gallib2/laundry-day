using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private const float RECYCLE_DISTANCE = 200f;
    [SerializeField] private MeshRenderer meshRenderer;

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }
    private void Update()
    {
        if(transform.position.z < Player.Instance.transform.position.z - RECYCLE_DISTANCE)
        {
            PlatformManager.Instance?.RecyclePlatform(this);
        }
    }
}
