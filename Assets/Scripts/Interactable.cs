using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Collider collider;
    private const float DYING_DISTANCE = 18f;
    private void Start()
    {
        collider.isTrigger = true;
    }

    public virtual void Interact()
    {
        Die();
    }

    //TODO: replace with a different condition - OnBecameInvisible seems to be making mistakes.
    /* private void OnBecameInvisible()
     {
        // Debug.Log("Interactable OnBecameInvisible");
         Die();
     }*/

    private void Update()
    {
        //TODO: no need to check every frame
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


