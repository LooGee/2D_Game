using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Stage_Link : MonoBehaviour
{

    // === 외부 파라미터（Inspector 표시） =====================
    public string linkSceneName;
    public string linkLabelName;

    public int linkDir = 0;
    public bool linkInput = true; // fales = AutoJump
    public float linkDelayTime = 0.1f;

    public bool sePlay = true;

    // === 내부 파라미터 ======================================
    Transform playerTrfm;
    PlayerController playerCtrl;

    // === 코드（Monobehaviour 기본 기능 구현） ================
    void Awake()
    {
        playerTrfm = PlayerController.GetTranform();
        playerCtrl = playerTrfm.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D_PlayerEvent(GameObject go)
    {
        if (!linkInput)
        {
            LinkScene();
        }
    }

    // === 코드（씬 점프 구현） ========================
    public void LinkScene()
    {
        // 점프할 곳을 설정
        if (linkSceneName == "")
        {
            linkSceneName = Application.loadedLevelName;

            // 체크 포인트
            PlayerController.checkPointEnabled = true;
            PlayerController.checkPointLabelName = linkLabelName;
            PlayerController.checkPointSceneName = linkSceneName;
            PlayerController.checkPointHp = PlayerController.nowHp;

            playerCtrl.ActionMove(0.0f);
            playerCtrl.activeSts = false;
            /*
            if (sePlay)
            {
                AppSound.instance.SE_OBJ_EXIT.Play();
            }
            */
            Invoke("linkWork", linkDelayTime);
        }
    }
        void linkWork()
        {
        Debug.Log("Scene");
            playerCtrl.activeSts = true;

            if (SceneManager.GetActiveScene().name == linkSceneName)
            {
                // 씬 안에서 점프
                GameObject[] stageLinkList = GameObject.FindGameObjectsWithTag("EventTrigger");
                foreach (GameObject stageLink in stageLinkList)
                {
                    if (stageLink.GetComponent<Stage_CheckPoint>().labelName == linkLabelName)
                    {
                        playerTrfm.position = stageLink.transform.position;
                        playerCtrl.groundY = playerTrfm.position.y;
                        Camera.main.transform.position = new Vector3(playerTrfm.position.x, playerTrfm.position.y, -10.0f);
                        break;
                    }
                }
            }
            else
            {
                // 씬 밖으로 점프
                SceneManager.LoadScene(linkSceneName);
            }
        }
    }

