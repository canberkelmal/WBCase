using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSc : MonoBehaviour
{
    [HideInInspector]
    public Material blockMaterial;

    private GameManager gM;


    void Awake()
    {
        gM = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
