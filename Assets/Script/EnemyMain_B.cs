using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain_B : EnemyMain
{
    // === 외부 파라미터（Inspector 표시） =====================
    public int aiIfRUNTOPLAYER = 25;
    public int aiIfESCAPE = 5;
    public int aiIfRETURNTODOGPILE = 10;

    public int damageAttack_A = 1;

    int actionNum;
    public override void FixedUpdateAI()
    {
        // AI 스테이트
        //Debug.Log (string.Format(">>> aists {0}",aiState));
        switch (aiState)
        {
            case ENEMYAISTS.ACTIONSELECT: // 사고 루틴 기점
                                          // 액션 선택
                int n = SelectRandomAIState();
                if (n < aiIfRUNTOPLAYER)
                {
                    SetAIState(ENEMYAISTS.RUNTOPLAYER, 3.0f);
                    actionNum = Random.Range(1, 10);
                }
                else
                if (n < aiIfRUNTOPLAYER + aiIfESCAPE)
                {
                    SetAIState(ENEMYAISTS.ESCAPE, Random.Range(2.0f, 5.0f));
                }
                else
                    if (n < aiIfRETURNTODOGPILE + aiIfESCAPE + aiIfRETURNTODOGPILE)
                {
                    if (dogPile != null)
                    {
                        SetAIState(ENEMYAISTS.RETURNTODOGPILE, 2.0f);
                    }
                }
                else
                {
                    SetAIState(ENEMYAISTS.WAIT, 1.0f + Random.Range(0.0f, 1.0f));
                }

                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.WAIT: // 휴식
                enemyCtrl.ActionLookup(player, 0.1f);
                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.RUNTOPLAYER: // 가까이 다가간다              
                if (!enemyCtrl.ActionMoveToNear(player, 1.5f))
                {
                    if (actionNum < 6)
                    {
                        Attack_A();
                    }
                    else
                        Attack_B();

                }
                break;

            case ENEMYAISTS.ESCAPE: // 멀어진다
                if (!enemyCtrl.ActionMoveToFar(player, 4.0f))
                {
                    SetAIState(ENEMYAISTS.ACTIONSELECT, 1.0f);
                }
                break;

            case ENEMYAISTS.RETURNTODOGPILE:
                if (enemyCtrl.ActionMoveToNear(dogPile, 3.0f))
                {
                    if (GetDistanePlayer() < 2.0f)
                    {
                        Attack_A();
                    }
                    else
                    {
                        SetAIState(ENEMYAISTS.ACTIONSELECT, 1.0f);
                    }
                }
                break;
        }
    }

    void Attack_A()
    {
        enemyCtrl.ActionLookup(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_A", damageAttack_A);
        // enemyCtrl.attackNockBackVector = new Vector2(1.0f, 2.0f);
        SetAIState(ENEMYAISTS.WAIT, 2.0f);
    }

    void Attack_B()
    {
        enemyCtrl.ActionLookup(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_B", damageAttack_A);
        //enemyCtrl.attackNockBackVector = new Vector2(1.0f, 2.0f);
        SetAIState(ENEMYAISTS.WAIT, 2.0f);
    }
}

