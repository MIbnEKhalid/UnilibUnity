using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PrefabManagerAssigment : MonoBehaviour
{
    [System.Serializable]
    public class PrefabData
    {
        public string assigNDate;  // Map issueDate in JSON to assignDate in the C# class
        public string dueDate;  // Due date matches the JSON key name
        public string subject;  // Subject matches the JSON key name
        public string detail;  // Detail matches the JSON key name
    }

    public GameObject prefabTemplate;   // Prefab template
    public Transform parentContainer;   // Parent transform for instantiated prefabs
    public string jsonFileName = "assigment.json"; // Name of the JSON file in Assets folder
    private void Start()
    {
        LoadDataFromJson();
    }

    private void LoadDataFromJson()
    {
        // Load JSON file from Resources folder
        string filePath = Path.Combine(Application.dataPath, jsonFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"JSON file not found at path: {filePath}");
            return;
        }

        string jsonData = File.ReadAllText(filePath);
        // Deserialize the JSON data into a list of PrefabData
        List<PrefabData> dataList = JsonConvert.DeserializeObject<List<PrefabData>>(jsonData);

        if (dataList == null || dataList.Count == 0)
        {
            Debug.LogError("No items found in JSON data.");

            return;
        }

        float xOffset = 0f;
        float yOffset = 0f;

        foreach (PrefabData data in dataList)
        {
            // Instantiate the prefab
            GameObject newPrefab = Instantiate(prefabTemplate, parentContainer);


            // Set the position with 20 x gap
            newPrefab.transform.localPosition = new Vector3(xOffset, yOffset, 0);
        //    xOffset -= 120f; // Adjust gap between prefabs as needed
            yOffset -= 120f; // Adjust gap between prefabs as needed

            // Initialize the prefab with data
            if (newPrefab.TryGetComponent(out PrefabControllerAssigment controller))
            {
                controller.Initialize(data.subject + ": " + data.detail, data.assigNDate, data.dueDate);
            }
            else
            {
                Debug.LogError("PrefabControllerAssigment component not found on instantiated prefab.");
            }
        }
    }
}
