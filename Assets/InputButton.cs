using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputButton : MonoBehaviour
{
    [SerializeField] InputType inputType;
    public InputType InputType
    {
        get { return inputType; }
    }
}
