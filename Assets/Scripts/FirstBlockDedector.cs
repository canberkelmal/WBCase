using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBlockDedector : MonoBehaviour
{
    public GameManager gameManager;

    public Transform DetectFirstBlock()
    {
        Ray ray = new(transform.position, transform.up);
        if (Physics.Raycast(ray, out RaycastHit hitUp, 1, gameManager.blocksLayerMask))
        {
            if (hitUp.transform != null)
            {
                return hitUp.transform;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
