using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class shareScript : MonoBehaviour
{
    string currentScene;

    // Start is called before the first frame update
    public void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && currentScene != "menu")
        {
            Debug.Log("Back to main menu");
            SceneManager.LoadScene("menu");
        }
    }
}
