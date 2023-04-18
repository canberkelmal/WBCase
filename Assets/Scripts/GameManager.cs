using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine;
using static System.Net.WebRequestMethods;
using UnityEditor.Rendering.LookDev;
using System;

public class GameManager : MonoBehaviour
{
    // declare a two-dimensional array to store materials
    public Material[][] blockMaterials;
    public int M, N, A, B, C;
    public Transform[,] blocksArray;
    public int[] groupMembersCount = new int[0];
    public FirstBlockDedector dedectorSc;
    public LayerMask blocksLayerMask;
    public int groupIndex = 1;

    // declare an array of strings to store the names of the color folders
    private string[] colorFolderNames = { "Blue", "Green", "Pink", "Purple", "Red", "Yellow" };

    // declare an array of strings to store the names of the material files
    private string[] materialNames = { "A", "B", "C", "D" };

    private GameObject clickedBlock;


    private void Awake()
    {
        groupIndex = 1;
        InitializeBlockMaterials();
        IncreaseGroupMembersCountLength();
        IncreaseGroupMembersCountLength();
    }

    private void Start()
    {
        blocksArray = new Transform[M, N];
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
        blocksArray[9, 0].GetComponent<BlockSc>().row = 9;
        blocksArray[9, 0].GetComponent<BlockSc>().column = 0;
        for (int i = M-1; i >= 0; i--)
        {
            // set first blocks of the rows except first row
            if (i < M-1)
            {
                Ray ray = new Ray(blocksArray[i + 1, 0].position, -blocksArray[i + 1, 0].up);
                if (Physics.Raycast(ray, out RaycastHit hitUp, 1, blocksLayerMask))
                {
                    if (hitUp.transform != null)
                    {
                        blocksArray[i, 0] = hitUp.transform;
                        hitUp.transform.GetComponent<BlockSc>().row = i;
                        hitUp.transform.GetComponent<BlockSc>().column = 0;
                    }
                }
            }

            // set remaining blocks of the rows
            for (int j = 1; j < N; j++)
            {
                Ray ray2 = new Ray(blocksArray[i, j - 1].position, -blocksArray[i, j - 1].right);
                if (Physics.Raycast(ray2, out RaycastHit hitRight, 1, blocksLayerMask))
                {
                    if (hitRight.transform != null)
                    {
                        blocksArray[i, j] = hitRight.transform;
                        hitRight.transform.GetComponent<BlockSc>().row = i;
                        hitRight.transform.GetComponent<BlockSc>().column = j;
                    }
                }

            }
        }
        Invoke("SetBlocksGroupIndexes", 0.1f);
    }

    private void SetBlocksGroupIndexes()
    {
        for (int i = M - 1; i >= 0; i--)
        {
            for (int j = 0; j < N; j++)
            {
                BlockSc currentBlockSc = blocksArray[i, j].GetComponent<BlockSc>();
                BlockSc downBlockSc = i != 9 ? blocksArray[i + 1, j].GetComponent<BlockSc>() : null;
                BlockSc leftBlockSc = j != 0 ? blocksArray[i, j - 1].GetComponent<BlockSc>() : null;

                print("current: " + blocksArray[i, j].name);

                if (leftBlockSc != null)
                    print("left: " + blocksArray[i, j - 1].name);

                if (downBlockSc != null)
                    print("down: " + blocksArray[i + 1, j].name);


                // if leftBlock color same with currentBlock
                if (j != 0 && currentBlockSc.colorIndex == leftBlockSc.colorIndex)
                {
                    // if both do not have a group
                    if(leftBlockSc.groupIndex == 0 && currentBlockSc.groupIndex == 0)
                    {
                        groupIndex++;
                        IncreaseGroupMembersCountLength();
                        leftBlockSc.groupIndex = groupIndex;
                        currentBlockSc.groupIndex = groupIndex;
                        AddMemberToGroupArray(groupIndex, 2);
                    }
                    // if leftBlock does not have a group and current has a group
                    else if (leftBlockSc.groupIndex == 0 && currentBlockSc.groupIndex != 0)
                    {
                        leftBlockSc.groupIndex = currentBlockSc.groupIndex;
                        AddMemberToGroupArray(currentBlockSc.groupIndex, 1);
                    }
                    // if leftBlock has a group and current does not have a group
                    else if (leftBlockSc.groupIndex != 0 && currentBlockSc.groupIndex == 0)
                    {
                        currentBlockSc.groupIndex = leftBlockSc.groupIndex;
                        AddMemberToGroupArray(currentBlockSc.groupIndex, 1);
                    }
                    // if both have a group
                    else if(leftBlockSc.groupIndex != 0 && currentBlockSc.groupIndex != 0 && leftBlockSc.groupIndex != currentBlockSc.groupIndex)
                    {
                        SetPreviousGroupIndex(leftBlockSc.groupIndex, currentBlockSc.groupIndex);
                    }
                }

                // if downBlock color same with currentBlock
                if (i != M-1 && currentBlockSc.colorIndex == downBlockSc.colorIndex)
                {
                    // if both do not have a group
                    if (downBlockSc.groupIndex == 0 && currentBlockSc.groupIndex == 0)
                    {
                        groupIndex++;
                        IncreaseGroupMembersCountLength();
                        downBlockSc.groupIndex = groupIndex;
                        currentBlockSc.groupIndex = groupIndex;
                        AddMemberToGroupArray(groupIndex, 2);
                    }
                    // if downBlock does not have a group and current has a group
                    else if (downBlockSc.groupIndex == 0 && currentBlockSc.groupIndex != 0)
                    {
                        downBlockSc.groupIndex = currentBlockSc.groupIndex;
                        AddMemberToGroupArray(currentBlockSc.groupIndex, 1);
                    }
                    // if downBlock has a group and current does not have a group
                    else if (downBlockSc.groupIndex != 0 && currentBlockSc.groupIndex == 0)
                    {
                        currentBlockSc.groupIndex = downBlockSc.groupIndex;
                        AddMemberToGroupArray(currentBlockSc.groupIndex, 1);
                    }
                    // if both have a group
                    else if (downBlockSc.groupIndex != 0 && currentBlockSc.groupIndex != 0 && downBlockSc.groupIndex != currentBlockSc.groupIndex)
                    {
                        SetPreviousGroupIndex(downBlockSc.groupIndex, currentBlockSc.groupIndex);
                    }
                }


                if (j != 0 && currentBlockSc.colorIndex != leftBlockSc.colorIndex && i != M - 1 && currentBlockSc.colorIndex != downBlockSc.colorIndex)
                {
                    //leftBlockSc.groupIndex = 0;
                    //downBlockSc.groupIndex = 0;
                    //currentBlockSc.groupIndex = 0;
                }
            }
        }
        SetIcons();
    }

    void AddMemberToGroupArray(int aa, int count)
    {
        groupMembersCount[aa] += count;
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
        for (int i = 0; i < M ; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if(blocksArray[i, j].GetComponent<BlockSc>().groupIndex == checkIndex)
                {
                    blocksArray[i, j].GetComponent<BlockSc>().groupIndex = newIndex;
                    AddMemberToGroupArray(newIndex, 1);
                }
            }
        }
    }

    void SetIcons()
    {
        for (int i = M - 1; i >= 0; i--)
        {
            for (int j = 0; j < N; j++)
            {
                if(blocksArray[i, j].GetComponent<BlockSc>().groupIndex > 0)
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
            clickedBlock = hit.transform.gameObject;
            if(clickedBlock.GetComponent<BlockSc>().groupIndex > 0)
            {
                DestroyClickedGroup(clickedBlock.GetComponent<BlockSc>().groupIndex);
            }
        }
    }
    void DestroyClickedGroup(int destroyedGroupInd)
    {
        for (int i = 0; i < M; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (blocksArray[i, j] != null && blocksArray[i, j].GetComponent<BlockSc>().groupIndex == destroyedGroupInd)
                {
                    Destroy(blocksArray[i, j].gameObject);
                }
            }
        }
    }

    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    /*
        private void SetBlocksGroupIndexes2()
        {
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    BlockSc currentBlockSc = blocksArray[i, j].GetComponent<BlockSc>();
                    BlockSc upBlockSc = i != 0 ? blocksArray[i - 1, j].GetComponent<BlockSc>() : null;
                    BlockSc rightBlockSc = j != 9 ? blocksArray[i, j + 1].GetComponent<BlockSc>() : null;

                    if (j != 9 && currentBlockSc.colorIndex == rightBlockSc.colorIndex)
                    {
                        // if both do not have a group
                        if (rightBlockSc.groupIndex == 0 && currentBlockSc.groupIndex == 0)
                        {
                            groupIndex++;
                            IncreaseGroupMembersCountLength();
                            rightBlockSc.groupIndex = groupIndex;
                            currentBlockSc.groupIndex = groupIndex;
                            AddMemberToGroupArray(groupIndex, 2);
                        }
                        // if leftBlock does not have a group and current has a group
                        else if (rightBlockSc.groupIndex == 0 && currentBlockSc.groupIndex != 0)
                        {
                            rightBlockSc.groupIndex = currentBlockSc.groupIndex;
                            AddMemberToGroupArray(currentBlockSc.groupIndex, 1);
                        }
                        // if leftBlock has a group and current does not have a group
                        else if (rightBlockSc.groupIndex != 0 && currentBlockSc.groupIndex == 0)
                        {
                            currentBlockSc.groupIndex = rightBlockSc.groupIndex;
                            AddMemberToGroupArray(currentBlockSc.groupIndex, 1);
                        }
                        // if both have a group
                        else if (rightBlockSc.groupIndex != 0 && currentBlockSc.groupIndex != 0)
                        {
                            SetPreviousGroupIndex(rightBlockSc.groupIndex, currentBlockSc.groupIndex);
                        }
                    }

                    if (i != 0 && currentBlockSc.colorIndex == upBlockSc.colorIndex)
                    {
                        // if both do not have a group
                        if (upBlockSc.groupIndex == 0 && currentBlockSc.groupIndex == 0)
                        {
                            groupIndex++;
                            IncreaseGroupMembersCountLength();
                            upBlockSc.groupIndex = groupIndex;
                            currentBlockSc.groupIndex = groupIndex;
                            AddMemberToGroupArray(groupIndex, 2);
                        }
                        // if leftBlock does not have a group and current has a group
                        else if (upBlockSc.groupIndex == 0 && currentBlockSc.groupIndex != 0)
                        {
                            upBlockSc.groupIndex = currentBlockSc.groupIndex;
                            AddMemberToGroupArray(currentBlockSc.groupIndex, 1);
                        }
                        // if leftBlock has a group and current does not have a group
                        else if (upBlockSc.groupIndex != 0 && currentBlockSc.groupIndex == 0)
                        {
                            currentBlockSc.groupIndex = upBlockSc.groupIndex;
                            AddMemberToGroupArray(currentBlockSc.groupIndex, 1);
                        }
                        // if both have a group
                        else if (upBlockSc.groupIndex != 0 && currentBlockSc.groupIndex != 0)
                        {
                            SetPreviousGroupIndex(upBlockSc.groupIndex, currentBlockSc.groupIndex);
                        }
                    }

                }
            }
            SetGroupCounts();
        }
    */
}
