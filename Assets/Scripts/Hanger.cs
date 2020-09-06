using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanger : MonoBehaviour
{
    private const float DYING_DISTANCE = 48f;

    private void Update()
    {
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
