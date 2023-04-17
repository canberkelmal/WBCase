using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine;
using static System.Net.WebRequestMethods;
using UnityEditor.Rendering.LookDev;

public class GameManager : MonoBehaviour
{
    // declare a two-dimensional array to store materials
    public Material[][] blockMaterials;
    public int M, N, A, B, C;
    public Transform[,] blocksArray;
    public int[] groupMembersCount = new int[0];
    public FirstBlockDedector dedectorSc;
    public LayerMask blocksLayerMask;
    public int groupIndex = 0;

    // declare an array of strings to store the names of the color folders
    private string[] colorFolderNames = { "Blue", "Green", "Pink", "Purple", "Red", "Yellow" };

    // declare an array of strings to store the names of the material files
    private string[] materialNames = { "A", "B", "C", "D" };

    private GameObject clickedBlock;


    private void Awake()
    {
        InitializeBlockMaterials();
        blocksArray = new Transform[M, N];
    }

    private void Start()
    {
        FillBlocksArray();
    }

    // Update is called once per frame
    void Update()
    {
        // Restart the game when the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }


        if (Input.GetMouseButtonUp(0))
        {
            ClickDedector();
        }
    }
    
    private void FillBlocksArray()
    {
        blocksArray[9,0] = dedectorSc.DetectFirstBlock();
        for (int i = M-1; i >= 0; i--)
        {
            // set first blocks of the rows except first row
            if (i < M-1)
            {
                Ray ray = new Ray(blocksArray[i + 1, 0].localPosition, blocksArray[i + 1, 0].up);
                if (Physics.Raycast(ray, out RaycastHit hitUp, 1, blocksLayerMask))
                {
                    if (hitUp.transform != null)
                    {
                        blocksArray[i, 0] = hitUp.transform;
                    }
                }
            }

            // set remaining blocks of the rows
            for (int j = 1; j < N; j++)
            {
                if(j>0)
                {

                }
                print(i + " i-j " + j + " pos: " + blocksArray[i, j - 1].position);
                Ray ray2 = new Ray(blocksArray[i, j - 1].localPosition, blocksArray[i, j - 1].right);
                if (Physics.Raycast(ray2, out RaycastHit hitRight, 1, blocksLayerMask))
                {
                    print(hitRight.transform);
                    if (hitRight.transform != null)
                    {
                        print(i + " i-j " + j + " filled");
                        blocksArray[i, j] = hitRight.transform;
                    }
                }

            }
        }
        SetBlocksGroupIndexes();
    }

    private void SetBlocksGroupIndexes()
    {
        for (int i = M - 1; i >= 0; i--)
        {
            for (int j = 0; j < N; j++)
            {
                BlockSc currentBlockSc = blocksArray[i, j].GetComponent<BlockSc>();
                BlockSc leftBlockSc = blocksArray[i, j - 1].GetComponent<BlockSc>();
                BlockSc downBlockSc = blocksArray[i + 1, j].GetComponent<BlockSc>();

                // if leftBlock color same with currentBlock
                if (j != 0 && currentBlockSc.colorIndex == leftBlockSc.colorIndex)
                {
                    // if leftBlock does not have a group
                    if(leftBlockSc.groupIndex == 0)
                    {
                        groupIndex++;
                        IncreaseGroupMembersCountLength();
                        leftBlockSc.groupIndex = groupIndex;
                        currentBlockSc.groupIndex = groupIndex;
                    }
                    else
                    {
                        currentBlockSc.groupIndex = leftBlockSc.groupIndex;
                    }
                }

                // if downBlock color same with currentBlock
                if (i != 9 && currentBlockSc.colorIndex == downBlockSc.colorIndex)
                {
                    // if downBlock does not have a group
                    if (downBlockSc.groupIndex == 0 && currentBlockSc.colorIndex == 0)
                    {
                        groupIndex++;
                        IncreaseGroupMembersCountLength();
                        leftBlockSc.groupIndex = groupIndex;
                        currentBlockSc.groupIndex = groupIndex;
                    }
                    // if downBlock does not have a group and current has a group
                    else if (downBlockSc.groupIndex == 0 && currentBlockSc.colorIndex != 0)
                    {
                        downBlockSc.groupIndex = currentBlockSc.groupIndex;
                    }
                    // if downBlock has a group and current does not have a group
                    else if (downBlockSc.groupIndex != 0 && currentBlockSc.colorIndex == 0)
                    {
                        currentBlockSc.groupIndex = downBlockSc.groupIndex;
                    }
                    // if both have a group
                    else
                    {
                        SetPreviousGroupIndex(downBlockSc.groupIndex, currentBlockSc.groupIndex);
                    }
                }
            }
        }
        SetGroupCounts();
    }

    void IncreaseGroupMembersCountLength()
    {
        int currentLength = groupMembersCount.Length;

        int[] newArr = new int[currentLength + 1];

        for (int i = 0; i < currentLength; i++)
        {
            newArr[i] = groupMembersCount[i];
        }

        newArr[currentLength] = 0;

        groupMembersCount = newArr;
    }

    void SetPreviousGroupIndex(int checkIndex, int newIndex)
    {
        for (int i = M - 1; i >= 0; i--)
        {
            for (int j = 0; j < N; j++)
            {
                if(blocksArray[i, j].GetComponent<BlockSc>().groupIndex == checkIndex)
                {
                    blocksArray[i, j].GetComponent<BlockSc>().groupIndex = newIndex;
                }
            }
        }
    }

    void SetGroupCounts()
    {
        for (int i = M - 1; i >= 0; i--)
        {
            for (int j = 0; j < N; j++)
            {
                if(blocksArray[i, j].GetComponent<BlockSc>().groupIndex != 0)
                {
                    groupMembersCount[blocksArray[i, j].GetComponent<BlockSc>().groupIndex]++;
                }                
            }
        }
        SetIcons();
    }

    void SetIcons()
    {
        for (int i = M - 1; i >= 0; i--)
        {
            for (int j = 0; j < N; j++)
            {
                int currentGroupCount = groupMembersCount[blocksArray[i, j].GetComponent<BlockSc>().groupIndex];
                if (currentGroupCount >= 0 && currentGroupCount <= A)
                {
                    blocksArray[i, j].GetComponent<BlockSc>().SetBlockIcon(3);
                }
                else if (currentGroupCount > A && currentGroupCount <= B)
                {
                    blocksArray[i, j].GetComponent<BlockSc>().SetBlockIcon(0);
                }
                else if (currentGroupCount > B && currentGroupCount <= C)
                {
                    blocksArray[i, j].GetComponent<BlockSc>().SetBlockIcon(1);
                }
                else if (currentGroupCount > C)
                {
                    blocksArray[i, j].GetComponent<BlockSc>().SetBlockIcon(2);
                }
            }
        }
    }


    // initialize the blockMaterials array with the materials in the color folders
    private void InitializeBlockMaterials()
    {
        blockMaterials = new Material[6][];
        for (int i = 0; i < colorFolderNames.Length; i++)
        {
            blockMaterials[i] = new Material[4];
            for (int j = 0; j < materialNames.Length; j++)
            {
                string path = "Materials/BlockMaterials/" + colorFolderNames[i] + "/" + materialNames[j]; 

                Material material = Resources.Load<Material>(path);
                //print(material);

                blockMaterials[i][j] = material;
            }
        }
    }

    public void ClickDedector()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 80))
        {
            Debug.Log(hit.transform.gameObject.GetComponent<Renderer>().material.name);

            clickedBlock = hit.transform.gameObject;

            clickedBlock.GetComponent<BlockSc>().CheckAround();
            //Destroy(clickedBlock);

        }
    }
    
    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
