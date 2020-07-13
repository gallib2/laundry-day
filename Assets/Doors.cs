using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float doorOpeningTriggerDistance = 50f;
     private bool _isOpen;
    private bool IsOpen
    {
        get { return _isOpen; }
        set
        {
            _isOpen = value;
            animator.SetBool("IsOpen", value);
        }
    }

    public void MoveToNewLocation(Vector3 newLocation)
    {
        transform.position = newLocation;
        IsOpen = false;
    }

    private void Update()
    {
        if (!IsOpen)
        {
            if (transform.position.z - Player.Instance.transform.position.z < doorOpeningTriggerDistance)
            {
                IsOpen = true;
            }
        }

    }
}
