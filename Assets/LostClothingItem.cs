using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostClothingItem : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Animator animator;


    public void Spawn(ClothingItemPhysicalProperties physicalProperties)
    {
        meshFilter.mesh = physicalProperties.mesh;
        meshRenderer.material = physicalProperties.material;
        animator.SetTrigger("Play");
        Invoke("Die", 3f);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

public struct ClothingItemPhysicalProperties
{
    public Mesh mesh;
    public Material material;
    public ClothingItemPhysicalProperties(Mesh mesh, Material material)
    {
        this.mesh = mesh;
        this.material = material;
    }
}