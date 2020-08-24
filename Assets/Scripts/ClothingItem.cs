using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClothingType : byte
{
    WHITES = 0, COLORS = 1, /*DELICATES = 2,*/ LENGTH = 2
}

public class ClothingItem : Interactable
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Transform graphicsParent;
    public Material Material
    {
        get { return meshRenderer.material; }
    }
    public Mesh Mesh
    {
        get { return meshFilter.mesh; }
    }

    [SerializeField] private ClothingType clothingType;
    public ClothingType ClothingType
    {
        get { return this.clothingType; }
    }

    public void ChangeType(ClothingType type, Material material, Mesh mesh, Quaternion angle)
    {
       
        clothingType = type;
        meshFilter.mesh = mesh;
        meshRenderer.material = material;
        graphicsParent.rotation = angle;
    }


}
