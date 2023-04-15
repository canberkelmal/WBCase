using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // declare a two-dimensional array to store materials
    public Material[][] blockMaterials;

    // declare an array of strings to store the names of the color folders
    private string[] colorFolderNames = { "Blue", "Green", "Pink", "Purple", "Red", "Yellow" };

    // declare an array of strings to store the names of the material files
    private string[] materialNames = { "A", "B", "C", "D" };

    private GameObject clickedBlock;

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
    
    
    
    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
