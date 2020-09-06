using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColourController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] Material material;
    private int currentIndex=0;
    private int nextIndex=1;
    private float t=0;
    [SerializeField] private float incrementPerSecond;
    [SerializeField] private Color[] colours;
    void Update()
    {
        if (t > 1)
        {
            t = 0;
            currentIndex++;
            nextIndex++;
            if (currentIndex>= colours.Length)
            {
                currentIndex = 0;
            }
            if (nextIndex >= colours.Length)
            {
                nextIndex = 0;
            }
        }
        Color newColour = Color.Lerp(colours[currentIndex], colours[nextIndex], t);
        material.color = newColour;
        R = newColour.r; G = newColour.g; B = newColour.b;
        t += incrementPerSecond * Time.deltaTime;

    }
    [SerializeField] public float R;
    [SerializeField] public float G;
    [SerializeField] public float B;
}
