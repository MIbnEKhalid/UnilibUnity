using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking; 
public class PrefabManagerAssigment : MonoBehaviour
{
    [System.Serializable]
    public class PrefabData
    {
        public string issueDate;  // Map issueDate in JSON to assignDate in the C# class
        public string dueDate;  // Due date matches the JSON key name
        public string subject;  // Subject matches the JSON key name
        public string description;  // Detail matches the JSON key name
    }

    public GameObject prefabTemplate;   // Prefab template
    public Transform parentContainer;   // Parent transform for instantiated prefabs
    public string jsonFileName = "assigment.json"; // Name of the JSON file in Assets folder
   
    private void Start()
    {
        LoadDataFromJsons();
    }

    public void RefreshAssigments()
    {
        // Clear all child objects of parentContainer
        foreach (Transform child in parentContainer)
        {
            Destroy(child.gameObject);
        }
        LoadDataFromJsons();

    }

    public void LoadDataFromJsons()
    {
        // GitHub raw URL for your JSON file
        string fileUrl = "https://raw.githubusercontent.com/MIbnEKhalid/Unilib.MIbnEKhalid.github.io/refs/heads/edit/assigmentsNquiz.json";

        StartCoroutine(FetchJsonFromUrl(fileUrl));
    }

    private IEnumerator FetchJsonFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error fetching JSON data: {request.error}");
            yield break;
        }

        string jsonData = request.downloadHandler.text;

        // Deserialize the JSON data into a list of PrefabData
        List<PrefabData> dataList = JsonConvert.DeserializeObject<List<PrefabData>>(jsonData);


        if (dataList == null || dataList.Count == 0)
        {
            Debug.LogError("No items found in JSON data.");
         }

        float yOffset = 0f;

        foreach (PrefabData data in dataList)
        {
            // Instantiate the prefab
            GameObject newPrefab = Instantiate(prefabTemplate, parentContainer);
            if (data == dataList[0])
            {
                RectTransform rectTransform = parentContainer as RectTransform;
                newPrefab.transform.localPosition = new Vector3(0, (rectTransform.rect.height / 2) - 90, 0);
                Debug.Log("First prefab");
            }
            // Set the position with 20 y gap
            newPrefab.transform.localPosition = new Vector3(0, yOffset, 0);
            yOffset -= 120f; // Adjust gap between prefabs as needed

            // Initialize the prefab with data
            if (newPrefab.TryGetComponent(out PrefabControllerAssigment controller))
            {
                controller.Initialize(data.subject + ": " + data.description, data.issueDate, data.dueDate);
            }
            else
            {
                Debug.LogError("PrefabControllerAssigment component not found on instantiated prefab.");
            }
        }

    }
    public void LoadDataFromJson()
    {
        // Load JSON file from Resources folder
        string filePath = "https://raw.githubusercontent.com/MIbnEKhalid/Unilib.MIbnEKhalid.github.io/edit/assigmentsNquiz.json";

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

        float yOffset = 0f;

        foreach (PrefabData data in dataList)
        {
            // Instantiate the prefab
            GameObject newPrefab = Instantiate(prefabTemplate, parentContainer);
            if (data == dataList[0])
            {
                RectTransform rectTransform = parentContainer as RectTransform;
                newPrefab.transform.localPosition = new Vector3(0, (rectTransform.rect.height / 2) - 90, 0);
                Debug.Log("First prefab");
            }
            // Set the position with 20 y gap
            newPrefab.transform.localPosition = new Vector3(0, yOffset, 0);
            yOffset -= 120f; // Adjust gap between prefabs as needed

            // Initialize the prefab with data
            if (newPrefab.TryGetComponent(out PrefabControllerAssigment controller))
            {
                controller.Initialize(data.subject + ": " + data.description, data.issueDate, data.dueDate);
            }
            else
            {
                Debug.LogError("PrefabControllerAssigment component not found on instantiated prefab.");
            }
        } 
     
    /*
     
        
                float yOffset = 0f;

        foreach (PrefabData data in dataList)
        {
            // Instantiate the prefab
            GameObject newPrefab = Instantiate(prefabTemplate, parentContainer);

            //if its first prefab set the Y position to 0
            if (data == dataList[0])
            {
                RectTransform rectTransform = parentContainer as RectTransform;
                newPrefab.transform.localPosition = new Vector3(0, (rectTransform.rect.height / 2)-90, 0);
                Debug.Log("First prefab");
            } 

            if (data != dataList[0])
            {
 
                if (data == dataList[1])
                {
                    yOffset -= -60; // Adjust gap between prefabs as needed
                }
                else
                {
                    yOffset -= 120f; // Adjust gap between prefabs as needed
                }

                newPrefab.transform.localPosition = new Vector3(0, yOffset, 0);

            }

            // Initialize the prefab with data
            if (newPrefab.TryGetComponent(out PrefabControllerAssigment controller))
            {
                controller.Initialize(data.subject + ": " + data.description, data.issueDate, data.dueDate);
            }
            else
            {
                Debug.LogError("PrefabControllerAssigment component not found on instantiated prefab.");
            }
        }
   
     
     */
     
    }
} 