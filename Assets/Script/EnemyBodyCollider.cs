using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyCollider : MonoBehaviour
{
    EnemyController enemyCtrl;
    Animator playerAnim;
    EnemyMain enemyMain;
    int attackHash = 0;

    void Awake()
    {
        enemyCtrl = GetComponentInParent<EnemyController>();
        enemyMain = GetComponentInParent<EnemyMain>();
        playerAnim = PlayerController.GetAnimator();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log ("Enemy OnTriggerEnter2D : " + other.name);

        if (other.tag == "PlayerArm")
        {
            AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
            if (attackHash != stateInfo.fullPathHash)
            {
                attackHash = stateInfo.fullPathHash;
                enemyCtrl.ActionDamage();
            }
        }
        else
        if (other.tag == "PlayerArmBullet")
        {
            Destroy(other.gameObject);
            enemyCtrl.ActionDamage();
        }
        else 
        if (other.tag == "DogPile")
        {
            enemyMain.SetAIState(ENEMYAISTS.RETURNTODOGPILE, 1.0f);
        }

    }

    void Update()
    {
        AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
        if (attackHash != 0 && stateInfo.fullPathHash == PlayerController.ANISTS_Idle)
        {
            attackHash = 0;
        }
    }
}
