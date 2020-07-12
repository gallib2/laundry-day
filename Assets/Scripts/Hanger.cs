using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanger : MonoBehaviour
{
    private const float DYING_DISTANCE = 18f;

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
        InteractablesManager.RecycleHanger(this);
    }
}
