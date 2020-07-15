using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClothingType : byte
{
    WHITES = 0, COLORS = 1, DELICATES = 2, LENGTH = 3
}

public class ClothingItem : Interactable
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Transform graphicsParent;

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
