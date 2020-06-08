using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage_CheckPoint : MonoBehaviour
{

    public string labelName = "";
   

    void OnTriggerEnter2D_PlayerEvent(GameObject go)
    {
        Debug.Log("Event");
        PlayerController.checkPointEnabled = true;
        PlayerController.checkPointLabelName = labelName;
        PlayerController.checkPointSceneName = SceneManager.GetActiveScene().name;

        PlayerController.checkPointHp = PlayerController.nowHp;
      
    }
}
