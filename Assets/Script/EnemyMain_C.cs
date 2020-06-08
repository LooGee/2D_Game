using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain_C : EnemyMain
{// === 외부 파라미터（Inspector 표시） =====================
    public int ATTACKONSIGHT = 30;

    public int damageAttack_A = 2;
    public int fireAttack_A = 2;


    int fireCountAttack_A = 1;


    public override void FixedUpdateAI()
    {
        // AI 스테이트
        //Debug.Log (string.Format(">>> aists {0}",aiState));
        switch (aiState)
        {
            case ENEMYAISTS.ACTIONSELECT: // 사고 루틴 기점
                                          // 액션 선택
                int n = SelectRandomAIState();
                if (n < ATTACKONSIGHT)
                {
                    SetAIState(ENEMYAISTS.ATTACKONSIGHT, 3.0f);

                }           
                else
                {
                    SetAIState(ENEMYAISTS.WAIT, 1.0f + Random.Range(0.0f, 2.0f));
                }
                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.WAIT: // 휴식
                enemyCtrl.ActionLookup(player, 0.1f);
                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.ATTACKONSIGHT: // 가까이 다가간다              
                if (GetDistanePlayerX() < 5f)
                {
                    Attack_A();
                }
                break;
        }
    }

    void Attack_A()
    {
        enemyCtrl.ActionLookup(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_A", damageAttack_A);

        fireCountAttack_A++;
        if (fireCountAttack_A >= fireAttack_A)
        {
            fireCountAttack_A = 0;


            SetAIState(ENEMYAISTS.WAIT, 2.0f);
        }
    }
}
