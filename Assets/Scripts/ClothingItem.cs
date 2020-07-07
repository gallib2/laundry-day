using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClothingType : byte
{
    WHITE = 0, DARK = 1, DELICATE = 2, LENGTH = 3
}

public class ClothingItem : Interactable
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private ClothingType clothingType;
    public ClothingType ClothingType
    {
        get { return this.clothingType; }
    }
}
