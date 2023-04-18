using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Net.WebRequestMethods;

public class BlockSc : MonoBehaviour
{
    //[HideInInspector]
    public Material blockMaterial;
    //[HideInInspector]
    public int colorIndex;
    [HideInInspector]
    public int iconIndex = 3;
    //[HideInInspector]
    public int groupIndex = 0;

    private GameManager gM;

    public int row;
    public int column;


    private void Awake()
    {
        gM = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        SetBlockRandom();
        transform.position += Vector3.back * transform.position.y * 0.01f;
    }

    public void SetBlockRandom()
    {
        colorIndex = Random.Range(0, 6);
        blockMaterial = gM.blockMaterials[colorIndex][iconIndex];
        gameObject.GetComponent<Renderer>().material = blockMaterial;
    }

    public void SetBlockIcon(int ind)
    {
        iconIndex = ind;
        blockMaterial = gM.blockMaterials[colorIndex][iconIndex];
        GetComponent<Renderer>().material = blockMaterial;
    }

    private void DrawRays()
    {
        Debug.DrawRay(transform.position, Vector3.right, Color.red);
        Debug.DrawRay(transform.position, Vector3.up, Color.red);
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        Debug.DrawRay(transform.position, Vector3.left, Color.red);
    }

}
