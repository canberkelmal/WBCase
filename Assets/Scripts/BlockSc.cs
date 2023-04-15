using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSc : MonoBehaviour
{
    [HideInInspector]
    public Material blockMaterial;

    [HideInInspector]
    public int colorIndex;

    private GameManager gM;




    private void Awake()
    { 
        gM = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        colorIndex = Random.Range(0, 6);
        blockMaterial = gM.blockMaterials[colorIndex][3];
        gameObject.GetComponent<Renderer>().material = blockMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
