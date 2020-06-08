using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FIREBULLET
{
    ANGLE,
    HOMING
}

public class FireBullet : MonoBehaviour
{

    // === 외부 파라미터（Inspector 표시） =====================
    public FIREBULLET fireType = FIREBULLET.HOMING;

    public float attackDamage = 1;
    public Vector2 attackNockBackVector;

    public bool penetration = false;

    public float lifeTime = 3.0f;
    public float speedV = 10.0f;
    public float speedA = 0.0f;
    public float angle = 0.0f;

    public float homingTime = 0.0f;


    public Sprite hiteSprite;
    public Vector3 hitEffectScale = Vector3.one;
    public float rotateVt = 360.0f;

    // === 외부 파라미터 ======================================
    [System.NonSerialized] public Transform ownwer;
    [System.NonSerialized] public GameObject targetObject;
    [System.NonSerialized] public bool attackEnabled;

    // === 내부 파라미터 ======================================
    float fireTime;
    float homingAngle; 
    float speed;

    // === 코드（Monobehaviour 기본 기능 구현） ================
    void Start()
    {
        targetObject = PlayerController.GetGameObject();

        switch (fireType)
        {
            case FIREBULLET.ANGLE:
                speed = (ownwer.localScale.x < 0.0f) ? -speedV : +speedV;
                GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0.0f, 0.0f, angle) * new Vector3(speed, 0.0f, 0.0f);
                break;
            case FIREBULLET.HOMING:    
                speed = speedV;
                Homing(1.0f);
                break;
        }

        fireTime = Time.fixedTime;
        homingAngle = angle;
       

        attackEnabled = true;
        Destroy(this.gameObject, lifeTime);

       
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 자기 자신에게 닿았는지 검사
        if ((other.isTrigger ||
             (ownwer.tag == "Player" && other.tag == "PlayerBody") ||
             (ownwer.tag == "Player" && other.tag == "PlayerArm") ||
             (ownwer.tag == "Player" && other.tag == "PlayerArmBullet") ||
             (ownwer.tag == "Enemy" && other.tag == "EnemyBody") ||
             (ownwer.tag == "Enemy" && other.tag == "EnemyArm") ||
             (ownwer.tag == "Enemy" && other.tag == "EnemyArmBullet")))
        {
            Debug.Log("THis");
            return;
        }

        // 벽에 닿았는지 검사
        GetComponent<SpriteRenderer>().sprite = hiteSprite;
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        transform.localScale = hitEffectScale;
        Destroy(this.gameObject, 0.1f);

    }

    void Update()
    {
   
    }

    void FixedUpdate()
    {
        if (fireType != FIREBULLET.ANGLE && (Time.fixedTime - fireTime) < homingTime)
        {
            Homing(Time.fixedDeltaTime);
        }
    }

    // === 코드（호밍 처리） =============================
    void Homing(float t)
    {
        Vector3 posTarget = targetObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);

        switch (fireType)
        {
            case FIREBULLET.HOMING:
                {
                    // 항상 완벽하게 호밍
                    Vector3 vecTarget = posTarget - transform.position;
                    Vector3 vecMove = (Quaternion.LookRotation(vecTarget) * Vector3.forward) * speed;
                    GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0.0f, 0.0f, angle) * vecMove;
                }
                break;
           
        }

        speed += speedA * Time.fixedDeltaTime;
    }
}