using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    EnemyMain enemyMain;

    void Awake()
    {
        enemyMain = GetComponent<EnemyMain>();
    }

    void OnWillRenderObject()
    {
        if (Camera.current.tag == "MainCamera")
        {
            enemyMain.cameraEnabled = true;
        }
    }
}
