using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : BaseCharacterController
{

    // === 외부 파라미터（Inspector 표시） =====================
    public float initHpMax = 15.0f;
    [Range(0.1f, 20.0f)] public float initSpeed = 5.0f;


    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Idle");
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Run");
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Jump");
    public readonly static int ANISTS_ATTACK_A = Animator.StringToHash("Base Layer.Attack");
    public readonly static int ANISTS_DEAD = Animator.StringToHash("Base Layer.Hurt");
    public readonly static int ANISTS_HURT = Animator.StringToHash("Base Layer.Dead");

    Image hpBar;

    public static bool checkPointEnabled = false;
    public static string checkPointSceneName = "";
    public static string checkPointLabelName = "";
    public static float checkPointHp = 0;

    public static bool initParam = true;

    int jumpCount = 0;

    volatile bool atkInputEnabled = false;

    bool breakEnabled = true;
    float groundFriction = 0.0f;

    public static float nowHpMax = 0;
    public static float nowHp = 0;

    [System.NonSerialized] public float groundY = 0.0f;
    [System.NonSerialized] public Vector3 enemyActiveZonePointA;
    [System.NonSerialized] public Vector3 enemyActiveZonePointB;


    // === 코드(지원 함수) 플레이어에게 접근하기====================
    public static GameObject GetGameObject()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
    public static Transform GetTranform()
    {
        return GameObject.FindGameObjectWithTag("Player").transform;
    }
    public static PlayerController GetController()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    public static Animator GetAnimator()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    protected override void Awake()
    {
        base.Awake();

        // 파라미터 초기화
        speed = initSpeed;
        SetHP(initHpMax, initHpMax);

        hpBar = GameObject.Find("Hp_Bar").GetComponent<Image>();

        BoxCollider2D boxCol2D = transform.Find("Collider_EnemyActiveZone").GetComponent<BoxCollider2D>();
        enemyActiveZonePointA = new Vector3(boxCol2D.offset.x - boxCol2D.size.x / 2.0f, boxCol2D.offset.y - boxCol2D.size.y / 2.0f);
        enemyActiveZonePointB = new Vector3(boxCol2D.offset.x + boxCol2D.size.x / 2.0f, boxCol2D.offset.y + boxCol2D.size.y / 2.0f);
        boxCol2D.transform.gameObject.SetActive(false);

        if (initParam)
        {
            SetHP(initHpMax, initHpMax);
            initParam = false;
        }
        if (SetHP(PlayerController.nowHp, PlayerController.nowHpMax))
        {
            // HP가 없을 때에는 1부터 시작
            SetHP(15, initHpMax);
        }

        // 체크 포인터에서부터 다시 시작
        if (checkPointEnabled)
        {
            Stage_CheckPoint[] triggerList = GameObject.Find("Stage").GetComponentsInChildren<Stage_CheckPoint>();
            foreach (Stage_CheckPoint trigger in triggerList)
            {
                if (trigger.labelName == checkPointLabelName)
                {
                    transform.position = trigger.transform.position;
                    groundY = transform.position.y;
                
                    break;
                }
            }
        }
        Camera.main.transform.position = new Vector3(transform.position.x,
                                                     groundY,
                                                     Camera.main.transform.position.z);
    }
    protected override void Update()
    {
        base.Update();

        hpBar.fillAmount = hp / hpMax;
    }

    protected override void FixedUpdateCharacter()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // 착지했는지 검사
        if (jumped)
        {
            if ((grounded && !groundedPrev))
            {
                Debug.Log("Jumped false");
                animator.SetTrigger("Idle");
                jumped = false;
                jumpCount = 0;
            }
        }
        if (!jumped)
        {
            jumpCount = 0;

        }
        if (stateInfo.fullPathHash == ANISTS_ATTACK_A)
        {
            speedVx = 0;
        }


        transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);

        // 점프 도중에 가로 이동 감속
        if (jumped && !grounded && groundCheck_OnMoveObject == null)
        {
            if (breakEnabled)
            {
                breakEnabled = false;
                speedVx *= 0.9f;
            }
        }
        if (breakEnabled)
        {
            speedVx *= groundFriction;
        }

        Camera.main.transform.position = transform.position - Vector3.forward + Vector3.up;
    }

    public void EnebleAttackInput()
    {
        atkInputEnabled = true;
    }


    public override void ActionMove(float n)
    {
        if (!activeSts)
        {
            return;
        }

        // 초기화
        float dirOld = dir;
        breakEnabled = false;

        float movSpeed = Mathf.Clamp(Mathf.Abs(n), -1.0f, 1.0f);
        animator.SetFloat("MovSpeed", movSpeed);


        // 이동 검사
        if (n != 0.0f)
        {
            // 이동            
            dir = Mathf.Sign(n);
            speedVx = initSpeed * movSpeed * dir;
        }
        else
        {
            breakEnabled = true;
        }

        // 그 자리에서 돌아봤는지 검사
        if (dirOld != dir)
        {
            breakEnabled = true;
        }
    }

    public void ActionAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.fullPathHash == ANISTS_Jump)
            return;
        if (stateInfo.fullPathHash != ANISTS_ATTACK_A || stateInfo.fullPathHash == ANISTS_Idle
            || stateInfo.fullPathHash == ANISTS_Run)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            if (atkInputEnabled)
                atkInputEnabled = false;
        }
    }

    public void ActionJump()
    {
        switch (jumpCount)
        {
            case 0:
                if (grounded)
                {
                    Debug.Log("Jump");
                    animator.SetTrigger("Jump");
                    GetComponent<Rigidbody2D>().velocity = Vector2.up * 10.0f;

                    jumped = true;
                    jumpCount++;
                }
                break;
            case 1:
                if (!grounded)
                {
                    animator.Play("Jump", 0, 0.0f);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 10.0f);
                    jumped = true;
                    jumpCount++;
                }
                break;
        }
    }


    public void ActionEtc()
    {
        Collider2D[] otherAll = Physics2D.OverlapPointAll(groundCheck_C.position);
        foreach (Collider2D other in otherAll)
        {
            if (other.tag == "EventTrigger")
            {
                Stage_Link link = other.GetComponent<Stage_Link>();
                if (link != null)
                {
                    link.LinkScene();
                }
            }
        }
    }

    public void ActionDamage(float damage)
    {
        // 피격 처리해도 되는지 검사
        if (!activeSts)
        {
            return;
        }
        Debug.Log("DMG");

        animator.SetTrigger("DMG_A");
        speedVx = 0;
      //  GetComponent<Rigidbody2D>().gravityScale = gravityScale;

        if (jumped)
        {
            damage *= 1.5f;
        }

        if (SetHP(hp - damage, hpMax))
        {
            Dead(true); // 사망
        }
    }

    // === 코드（그 외） ====================================
    public override void Dead(bool gameOver)
    {
        // 사망 처리해도 되는지 확인
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!activeSts || stateInfo.fullPathHash == ANISTS_DEAD)
        {
            return;
        }

        base.Dead(gameOver);

        if (gameOver)
        {
            SetHP(0, hpMax);
            Invoke("GameOver", 3.0f);
        }
        else
        {
            SetHP(hp / 2, hpMax);
            Invoke("GameReset", 3.0f);
        }


    }

    public void GameOver()
    {
      //  PlayerController.score = 0;
        PlayerController.nowHp = PlayerController.checkPointHp;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GameReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public override bool SetHP(float _hp, float _hpMax)
    {
        if (_hp > _hpMax)
        {
            _hp = _hpMax;
        }
        nowHp = _hp;
        nowHpMax = _hpMax;
        return base.SetHP(_hp, _hpMax);
    }
}




