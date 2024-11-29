using System.Collections;
using System.Collections.Generic; 
using UnityEngine; 
using TMPro;

public class PrefabControllerAssigment : MonoBehaviour
{ 
    public TMP_Text AssignDate;  
    public TMP_Text DueDate; 
    public TMP_Text Detail;  

    // Function to initialize the prefab with data
    public void Initialize(string detail, string assigndate, string duedate)
    { 
        AssignDate.text = assigndate;
        DueDate.text = duedate;
        Detail.text = detail;  
    }
}
