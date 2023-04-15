using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // declare a two-dimensional array to store materials
    [SerializeField] private Material[][] blockMaterials;

    // declare an array of strings to store the names of the color folders
    [SerializeField] private string[] colorFolderNames = { "Blue", "Green", "Pink", "Purple", "Red", "Yellow" };

    // declare an array of strings to store the names of the material files
    private string[] materialNames = { "A", "B", "C", "D" };

    private void Awake()
    {
        InitializeBlockMaterials();
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
                print(material);

                blockMaterials[i][j] = material;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
