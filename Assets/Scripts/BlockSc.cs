using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Net.WebRequestMethods;

public class BlockSc : MonoBehaviour
{
    [HideInInspector]
    public Material blockMaterial;
    [HideInInspector]
    public int colorIndex;
    [HideInInspector]
    public int iconIndex = 3;
    [HideInInspector]
    public bool isChecked = false;
    [HideInInspector]
    public bool isCounted = false;
    [HideInInspector]
    public bool isAdded = false;
    [HideInInspector]
    public GameObject[] sames;

    private GameManager gM;

    public float rayDistance = 200f;


    private void Awake()
    { 
        gM = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        iconIndex = 3;
        SetBlockRandom();
        transform.position += Vector3.back * transform.position.y * 0.01f;
        sames = new GameObject[1];
        sames[0] = gameObject;
        //CheckForIcon();
    }

    private void FixedUpdate()
    {
        //DrawRays();
    }

    public void SetBlockRandom()
    {
        colorIndex = Random.Range(0, 6);
        blockMaterial = gM.blockMaterials[colorIndex][iconIndex];
        gameObject.GetComponent<Renderer>().material = blockMaterial;
    }

    public void CheckForIcon()
    {
        if (!isCounted)
        {
            isCounted = true;
            Ray ray = new(transform.position, transform.up);
            if (Physics.Raycast(ray, out RaycastHit hitUp, rayDistance)) ;
            {
                if (hitUp.transform != null)
                {
                    if (hitUp.transform.CompareTag("Block"))
                    {
                        CheckTouchedBlockIcon(hitUp.transform.gameObject);
                    }
                }
            }

            ray = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(ray, out RaycastHit hitDown, rayDistance)) ;
            {
                if (hitDown.transform != null)
                {
                    if (hitDown.transform.CompareTag("Block"))
                    {
                        CheckTouchedBlockIcon(hitDown.transform.gameObject);
                    }
                }
            }

            ray = new Ray(transform.position, transform.right);
            if (Physics.Raycast(ray, out RaycastHit hitRight, rayDistance)) ;
            {
                if (hitRight.transform != null)
                {
                    if (hitRight.transform.CompareTag("Block"))
                    {
                        CheckTouchedBlockIcon(hitRight.transform.gameObject);
                    }
                }
            }

            ray = new Ray(transform.position, -transform.right);
            if (Physics.Raycast(ray, out RaycastHit hitLeft, rayDistance)) ;
            {
                if (hitLeft.transform != null)
                {
                    if (hitLeft.transform.CompareTag("Block"))
                    {
                        CheckTouchedBlockIcon(hitLeft.transform.gameObject);
                    }
                }
            }
            MarginSames();
            Invoke("SetIcon", Time.fixedDeltaTime);
            //SetIcon();
        }


    }

    private void MarginSames()
    {
        for(int i = 1; i < sames.Length; i++)
        {
            for(int j = 0; j < sames[i].GetComponent<BlockSc>().sames.Length; j++)
            {
                if (sames[i] != sames[i].GetComponent<BlockSc>().sames[j] && !sames[i].GetComponent<BlockSc>().sames[j].GetComponent<BlockSc>().isAdded)
                {
                    sames[i].GetComponent<BlockSc>().sames[j].GetComponent<BlockSc>().isAdded = true;
                    AddToSames(sames[i].GetComponent<BlockSc>().sames[j]);
                }
            }
        }
    }

    public void SetIcon()
    {
        int sameCount = sames.Length;
        if (sameCount >= 0 && sameCount <= gM.A)
        {            
            iconIndex = 3;
        }
        else if (sameCount > gM.A && sameCount <= gM.B)
        {
            iconIndex = 0;
        }
        else if (sameCount > gM.B && sameCount <= gM.C)
        {
            iconIndex = 1;
        }
        else if (sameCount > gM.C)
        {
            iconIndex = 2;
        }
        SetSamesIcons();
    }

    public void SetSamesIcons()
    {
        for(int i = 0; i < sames.Length; i++)
        {
            blockMaterial = gM.blockMaterials[colorIndex][iconIndex];
            sames[i].GetComponent<Renderer>().material = blockMaterial;
        }
    }

    public void CheckAround()
    {
        if(!isChecked)
        {
            isChecked = true;
            Ray ray = new(transform.position, transform.up);
            if (Physics.Raycast(ray, out RaycastHit hitUp, rayDistance)) ;
            {
                if (hitUp.transform != null)
                {
                    if (hitUp.transform.CompareTag("Block"))
                    {
                        CheckTouchedBlock(hitUp.transform.gameObject);
                    }
                }
            }

            ray = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(ray, out RaycastHit hitDown, rayDistance)) ;
            {
                if (hitDown.transform != null)
                {
                    if (hitDown.transform.CompareTag("Block"))
                    {
                        CheckTouchedBlock(hitDown.transform.gameObject);
                    }
                }
            }

            ray = new Ray(transform.position, transform.right);
            if (Physics.Raycast(ray, out RaycastHit hitRight, rayDistance)) ;
            {
                if (hitRight.transform != null)
                {
                    if (hitRight.transform.CompareTag("Block"))
                    {
                        CheckTouchedBlock(hitRight.transform.gameObject);
                    }
                }
            }

            ray = new Ray(transform.position, -transform.right);
            if (Physics.Raycast(ray, out RaycastHit hitLeft, rayDistance)) ;
            {
                if (hitLeft.transform != null)
                {
                    if (hitLeft.transform.CompareTag("Block"))
                    {
                        CheckTouchedBlock(hitLeft.transform.gameObject);
                    }
                }
            }

            if(sames.Length > 1)
            {
                DestroySames();
            }
            else
            {
                isChecked = false;
            }
        }
    }

    private void CheckTouchedBlockIcon(GameObject checkedBox)
    {
        if (checkedBox.GetComponent<BlockSc>().colorIndex == colorIndex)
        {
            AddToSames(checkedBox);
            checkedBox.GetComponent<BlockSc>().CheckForIcon();
        }
    }

    private void CheckTouchedBlock(GameObject checkedBox)
    {
        if(checkedBox.GetComponent<BlockSc>().colorIndex == colorIndex)
        {
            AddToSames(checkedBox);
            checkedBox.GetComponent<BlockSc>().CheckAround();
        }
    }

    private void AddToSames(GameObject addedBox)
    {
        GameObject[] temp = new GameObject[sames.Length + 1];
        temp[sames.Length] = addedBox;
        for (int i = 0; i < sames.Length; i++)
        {
            temp[i] = sames[i];
        }
        sames = temp;
    }

    private void DestroySames()
    {
        for(int i = 0;i < sames.Length; i++)
        {
            Destroy(sames[i]);
        }
    }


    private void DrawRays()
    {
        Debug.DrawRay(transform.position, Vector3.right, Color.red);
        Debug.DrawRay(transform.position, Vector3.up, Color.red);
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        Debug.DrawRay(transform.position, Vector3.left, Color.red);
    }

}
