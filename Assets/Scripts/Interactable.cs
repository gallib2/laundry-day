using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private const float DYING_DISTANCE = 18f;
    [SerializeField] private Collider collider;

    private void Start()
    {
        collider.isTrigger = true;
    }

    public virtual void Interact()
    {
        Die();
    }

    private void Update()
    {
        if (transform.position.z < Player.Instance.transform.position.z - DYING_DISTANCE)
        {
            Die();
        }
    }

    private void Die()
    {   
        InteractablesManager.RecycleInteractable(this);
    }
}


