using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseCharacterController
{

    // === 외부 파라미터（Inspector 표시） =====================
    public float initHpMax = 5.0f;
    public float initSpeed = 6.0f;
    public bool jumpActionEnabled = true;
    public Vector2 jumpPower = new Vector2(0.0f, 1500.0f);
    public Vector2 AttackjumpPower = new Vector2(0.0f, 700.0f);
    public int addScore = 500;
    [System.NonSerialized] public bool cameraRendered = false;
    [System.NonSerialized] public bool attackEnabled = false;
    [System.NonSerialized] public int attackDamage = 1;
    [System.NonSerialized] public Vector2 attackNockBackVector = Vector3.zero;

    // 애니메이션 해시 이름
    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Enemy_A_Idle");
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Enemy_A_Run");
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Enemy_A_Jump");
    public readonly static int ANITAG_ATTACK = Animator.StringToHash("Base Layer.Enemy_A_Attack");
    public readonly static int ANISTS_DMG_A = Animator.StringToHash("Base Layer.Enemy_DMG_A");
    public readonly static int ANISTS_Dead = Animator.StringToHash("Base Layer.Enemy_Dead");

    // === 캐시 ==========================================
    PlayerController playerCtrl;
    Animator playerAnim;

    // === 코드
    protected override void Awake()
    {
        base.Awake();

        playerCtrl = PlayerController.GetController();
        playerAnim = playerCtrl.GetComponent<Animator>();

        hpMax = initHpMax;
        hp = hpMax;
        speed = initSpeed;
    }

    protected override void FixedUpdateCharacter()
    {
        if (!cameraRendered)
        { return; }
        // 점프했는지 검사
        if (jumped)
        {
            // 착지 검사(A:접지 손간 판정 B:접지와 시간에 의한 판정)
            if ((grounded && !groundedPrev) || (grounded && Time.fixedTime > jumpStartTime + 1.0f))
            {
                jumped = false;
            }
            if (Time.fixedTime > jumpStartTime + 1.0f)
            {
                if (GetComponent<Rigidbody2D>().gravityScale < gravityScale)
                {
                    GetComponent<Rigidbody2D>().gravityScale = gravityScale;
                }
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().gravityScale = gravityScale;
        }

        // 캐릭터 방향
        transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);
        /*
        // Memo:공중에서 피격을 당했을 때 X방향 이동 금지
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.fullPathHash == EnemyController.ANISTS_DMG_A || 
            stateInfo.fullPathHash == EnemyController.ANISTS_Dead)
        {
            speedVx = 0.0f;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, GetComponent<Rigidbody2D>().velocity.y);
        }
        */
    }

    // === 코드（기본 액션） =============================
    public bool ActionJump()
    {
        if (jumpActionEnabled && grounded && !jumped)
        {
            animator.SetTrigger("Jump");
            GetComponent<Rigidbody2D>().AddForce(jumpPower);
            jumped = true;
            jumpStartTime = Time.fixedTime;
        }
        return jumped;
    }

    public void ActionAttack(string atkname, int damage)
    {
        attackEnabled = true;
        attackDamage = damage;
        animator.SetTrigger(atkname);
    }

    public void ActionDamage()
    {
        int damage = 0;
        if (hp <= 0)
        {
            return;
        }

        AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);

        if (grounded)
        {
            damage = 1;
            animator.SetTrigger("DMG_A");
            //	Debug.Log(string.Format(">>> DMG_A {0}",stateInfo.fullPathHash));

        }

        if (SetHP(hp - damage, hpMax))
        {
            Dead(false);
            /*
            int addScoreV = ((int)((float)addScore * (playerCtrl.hp / playerCtrl.hpMax))) * playerCtrl.comboCount;
            addScoreV = (int)((float)addScore * (grounded ? 1.0 : 1.5f));
            PlayerController.score += addScoreV;
            */

        }

    }
    // === 코드（그 외）====================================
    public override void Dead(bool gameOver)
    {
        base.Dead(gameOver);
        Destroy(gameObject, 1.0f);
    }



}

