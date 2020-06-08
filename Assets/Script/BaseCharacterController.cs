using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    // === 외부 파라미터(Inspector 표시) =====================
    public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);
    public Vector2 velocityMax = new Vector2(+100.0f, +50.0f);
    public GameObject[] fireObjectList;

    // === 외부 파라미터 ======================================
    [System.NonSerialized] public float hpMax = 10.0f;
    [System.NonSerialized] public float hp = 10.0f;
    [System.NonSerialized] public float dir = 1.0f;
    [System.NonSerialized] public float speed = 6.0f;
    [System.NonSerialized] public float basScaleX = 1.0f;
    [System.NonSerialized] public bool activeSts = false;
    [System.NonSerialized] public bool jumped = false;
    [System.NonSerialized] public bool grounded = false;
    [System.NonSerialized] public bool groundedPrev = false;

    // === 캐시 ==========================================
    [System.NonSerialized] public Animator animator;
    protected Transform groundCheck_L;
    protected Transform groundCheck_C;
    protected Transform groundCheck_R;

    // === 내부 파라미터 ======================================
    protected float speedVx = 0.0f;
    protected float speedVxAddPower = 0.0f;
    protected float gravityScale = 10.0f;
    protected float jumpStartTime = 0.0f;

    protected GameObject groundCheck_OnRoadObject;
    protected GameObject groundCheck_OnMoveObject;
    protected GameObject groundCheck_OnEnemyObject;


    protected bool addForceVxEnabled = false;
    protected float addForceVxStartTime = 0.0f;


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        groundCheck_L = transform.Find("GroundCheck_L");
        groundCheck_C = transform.Find("GroundCheck_C");
        groundCheck_R = transform.Find("GroundCheck_R");

        dir = (transform.localScale.x > 0.0f) ? 1 : -1;
        basScaleX = transform.localScale.x * dir;
        transform.localScale = new Vector3(basScaleX, transform.localScale.y, transform.localScale.z);

        activeSts = true;
        gravityScale = GetComponent<Rigidbody2D>().gravityScale;
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        // 구멍에 빠졌는지 검사
        if (transform.position.y < -30.0f)
        {
            if (activeSts)
            {
                Dead(false); // 사망
            }
        }

    }

    protected virtual void FixedUpdate()
    {
        // 지면 검사
        groundedPrev = grounded;
        grounded = false;

        groundCheck_OnRoadObject = null;
        groundCheck_OnMoveObject = null;
        groundCheck_OnEnemyObject = null;

        Collider2D[][] groundCheckCollider = new Collider2D[3][];
        groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_L.position);
        groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_C.position);
        groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position);


        foreach (Collider2D[] groundCheckList in groundCheckCollider)
        {
            foreach (Collider2D groundCheck in groundCheckList)
            {
                if (groundCheck != null)
                {
                    if (!groundCheck.isTrigger)
                    {
                        grounded = true;
                        if (groundCheck.tag == "Road")
                        {                          
                            groundCheck_OnRoadObject = groundCheck.gameObject;
                        }
                        else
                        if (groundCheck.tag == "MoveObject")
                        {
                            groundCheck_OnMoveObject = groundCheck.gameObject;
                        }
                        else
                        if (groundCheck.tag == "EnemyObject")
                        {
                            groundCheck_OnEnemyObject = groundCheck.gameObject;
                        }
                    }
                }
            }
        }

        // 캐릭터 개별 처리 실행
        //Debug.Log(string.Format("ChrCom FixedUpdate {0} {1}",name,grounded));
        FixedUpdateCharacter();

        // 이동 계산
        //Debug.Log (">>>> " + string.Format("speedVx {0} y {1} g{2}",speedVx,rigidbody2D.velocity.y,grounded));
        GetComponent<Rigidbody2D>().velocity = new Vector2(speedVx, GetComponent<Rigidbody2D>().velocity.y);

        // Veclocity 값 검사
        float vx = Mathf.Clamp(GetComponent<Rigidbody2D>().velocity.x, velocityMin.x, velocityMax.x);
        float vy = Mathf.Clamp(GetComponent<Rigidbody2D>().velocity.y, velocityMin.y, velocityMax.y);
        GetComponent<Rigidbody2D>().velocity = new Vector2(vx, vy);
    }
    protected virtual void FixedUpdateCharacter()
    {
    }

    // === 코드(애니메이션 이벤트용 코드) ===============
    public virtual void AddForceAnimatorVx(float vx) // 힘을 가로 방향으로 주는 이벤트
    {
        //Debug.Log (string.Format("--- AddForceAnimatorVx {0} ----------------",vx));
        if (vx != 0.0f)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(vx * dir, 0.0f));
            addForceVxEnabled = true;
            addForceVxStartTime = Time.fixedTime;
        }
    }

    public virtual void AddForceAnimatorVy(float vy) //힘을 세로 방향으로 주는 이벤트
    {
        //Debug.Log (string.Format("--- AddForceAnimatorVy {0} ----------------",vy));
        if (vy != 0.0f)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, vy));
            jumped = true;
            jumpStartTime = Time.fixedTime;
        }
    } 


    // === 코드（기본 액션） =============================
    public virtual void ActionMove(float n)
    {
        if (n != 0.0f)
        {
            dir = Mathf.Sign(n);
            speedVx = speed * n;
            animator.SetTrigger("Run");
        }
        else
        {
            speedVx = 0;
            animator.SetTrigger("Idle");
        }
    }
    
    public virtual void ActionFire()
    {

        Transform goFire = transform.Find("Enemy_C_FirePoint");
        foreach(GameObject fireObject in  fireObjectList)
        {
            GameObject go = Instantiate(fireObject, goFire.position, Quaternion.identity)
                as GameObject;
            go.GetComponent<FireBullet>().ownwer = transform;
        }
    }


    public virtual void ActionJump(float n)
    {
        if (n != 0.0f)
        {
            dir = Mathf.Sign(n);
            speedVx = speed * n;
            animator.SetTrigger("Run");
        }
        else
        {
           
            speedVx = 0;
            animator.SetTrigger("Idle");
        }
    }

    //죽었는지
    public virtual void Dead(bool gameOver)
    {
        animator.SetTrigger("Dead");
        activeSts = false;
    }
    //Hp계산
    public virtual bool SetHP(float _hp, float _hpMax)
    {
        hp = _hp;
        hpMax = _hpMax;
        return (hp <= 0);
    }

    public bool ActionLookup(GameObject go, float near)  //오브젝트 방향 쳐다보기
    {
        if (Vector3.Distance(transform.position, go.transform.position) > near)
        {
            dir = (transform.position.x < go.transform.position.x) ? +1 : -1;
            return true;
        }
        return false;
    }

    public bool ActionMoveToNear(GameObject go, float near) //오브젝트에 가까워지게
    {
        if (Vector3.Distance(transform.position, go.transform.position) > near)
        {
            ActionMove((transform.position.x < go.transform.position.x) ? +1f : -1f);
            return true;
        }
        return false;
    }

    public bool ActionMoveToFar(GameObject go, float far) //오브젝트에 멀어지게 
    {
        if (Vector3.Distance(transform.position, go.transform.position) < far)
        {
            ActionMove((transform.position.x > go.transform.position.x) ? +1.0f : -1.0f);
            return true;
        }
        return false;
    }
}
