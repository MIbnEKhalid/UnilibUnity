using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [System.Serializable]
    public class PrefabData
    {
        public string name;     // Title of the item
        public string imageURL; // Online URL for the image
        public string link;       // URL to open when button is clicked
    }

    public GameObject prefabTemplate;   // Prefab template
    public Transform parentContainer;   // Parent transform for instantiated prefabs
    public string jsonFileName = "data.json"; // Name of the JSON file in Assets folder

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
        PrefabDataList dataList = JsonUtility.FromJson<PrefabDataList>($"{{\"items\":{jsonData}}}");

        if (dataList == null || dataList.items == null || dataList.items.Count == 0)
        {
            Debug.LogError("No items found in JSON data.");
            return;
        }

        float yOffset = 0f;

        foreach (PrefabData data in dataList.items)
        {
            // Instantiate the prefab
            GameObject newPrefab = Instantiate(prefabTemplate, parentContainer);

            // Set the position with 20 x gap
            newPrefab.transform.localPosition = new Vector3(yOffset, 0, 0);
            yOffset -= 720f; // Adjust gap between prefabs as needed

            // Initialize the prefab with data
            if (newPrefab.TryGetComponent(out PrefabController controller))
            {
                // Initialize PrefabController with name, imageURL path, and link
                controller.Initialize(data.name, data.imageURL, data.link);
            }
            else
            {
                Debug.LogError("PrefabController component not found on instantiated prefab.");
            }
        }
    }

    // Helper class for JSON deserialization
    [System.Serializable]
    public class PrefabDataList
    {
        public List<PrefabData> items;
    }
}
