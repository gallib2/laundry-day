using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Collider collider;
    private void Start()
    {
        collider.isTrigger = true;
    }
    public virtual void Interact()
    {
        Die();
    }

    private void OnBecameInvisible()
    {
        Die();
    }

    private void Die()
    {
        InteractablesManager.RecycleInteractable(this);
    }
}


