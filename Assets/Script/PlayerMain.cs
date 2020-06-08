using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    // === 내부 파라미터 ==========================================
    PlayerController playerCtrl;

    // === 코드（Monobehaviour 기본기능 구현） ================
    // === 코드（Monobehaviour 기본기능 구현） ================
    void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!playerCtrl.activeSts)
        {
            return;
        }

        float joyMv = Input.GetAxis("Horizontal");

        playerCtrl.ActionMove(joyMv);

        if (Input.GetButtonDown("Jump"))
        {
            playerCtrl.ActionJump();
        }

        if(Input.GetButtonDown("Fire1"))
        {
            playerCtrl.ActionAttack();
        }
    }
}
