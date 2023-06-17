using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AreaNameDisplay : MonoBehaviour
{
    public NavMeshAgent agent; // Reference to the NavMeshAgent component
    public Text areaNameText; // Reference to the UI text element to display the area name
    public GameObject ToActive;

    private List<string> areaNames; // List to store the names of the areas from JSON file

    private void Start()
    {
        // Load the JSON file and retrieve the area names
        string filePath = "Assets/Scripts/Json/Area.json";
        string jsonString = File.ReadAllText(filePath);
        areaNames = JsonConvert.DeserializeObject<List<string>>(jsonString);
        Debug.Log("Area Names:" + areaNames);
    }

    private void Update()
    {
        // Check if the agent is currently on the NavMesh
        if (agent.isOnNavMesh)
        {
            // Perform a raycast from the agent's position downward
            RaycastHit hit;
            if (Physics.Raycast(agent.transform.position, Vector3.down, out hit))
            {
                // Check if the hit collider has a name
                if (!string.IsNullOrEmpty(hit.collider.gameObject.name))
                {
                    string areaName = hit.collider.gameObject.name;

                    // Check if the area name is in the list of area names from JSON
                    if (areaNames.Contains(areaName))
                    {
                        // Display the area name on the UI text element
                        ToActive.gameObject.SetActive(true);
                        areaNameText.text = areaName;
                        HideAreaNameAfterDelay(3f);
                    }
                    else
                    {
                        // Clear the UI text element if the area name is not in the JSON list
                        areaNameText.text = "";
                    }
                }
            }
        }
    }
    private IEnumerator HideAreaNameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ToActive.gameObject.SetActive(false);
    }
}
