using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetSkill : MonoBehaviour
{
    [SerializeField] float SkillSpeed = 1f;
    [SerializeField] float DestroyDelay = 0.5f;
    [SerializeField] int Damage = 10;
    [SerializeField] float SkillCoefficient = 0.05f; // 스킬계수
    [SerializeField] float AttackDistance = 5f; // 캐스트 거리
    [SerializeField] float BoxHeight = 0.5f; // 박스의 높이
    [SerializeField] float BoxWidth = 1f; // 박스의 너비
    Rigidbody2D MyRigidbody;
    GameObject Player;
    PlayerMovement PlayerMovement;
    PlayerStatus PlayerStatus;
    bool IsAttackDone = false;
    [SerializeField] bool CanHitMany = false;
    bool IsCrit = false;
    public float CoolDown = 3f;
    public int MPUse = 0;
    float XSpeed;
    GameObject TargetMonster;
    Vector3 TargetPosition;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        MyRigidbody = GetComponent<Rigidbody2D>();
        PlayerMovement = FindObjectOfType<PlayerMovement>();
        XSpeed = PlayerMovement.transform.localScale.x * SkillSpeed;
        Invoke("DestroySelf", DestroyDelay);
        FlipSprite();

        PlayerStatus = FindObjectOfType<PlayerStatus>();
        Damage = Mathf.CeilToInt(Damage * (1 + SkillCoefficient * PlayerStatus.PlayerATK)); // 데미지 공식: 스킬계수 * 플레이어ATK
        Damage = Mathf.FloorToInt(Damage * Random.Range(1.0f, 1.31f)); // 데미지 랜덤값: 계산된 데미지의 1 ~ 1.3배로 조정
        if (Random.value < PlayerStatus.PlayerCrit)
        {
            IsCrit = true;
            Damage *= 2; // 크리티컬 공식: 최종 데미지 * 2
        }

        CheckMonster(); // 시작 시 오브젝트 위치를 타겟 몬스터의 위치로 변경
    }

    void Update()
    {
        MyRigidbody.velocity = new Vector2(XSpeed, 0f);

        // Update에서 타겟 몬스터 위치로 지속적으로 갱신
        if (TargetMonster != null)
        {
            transform.position = TargetMonster.transform.position;
        }
        else
        {
            transform.position = TargetPosition;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsAttackDone)
        {
            return;
        }
        if (other is BoxCollider2D && other.gameObject.tag == "Monster")
        {
            BasicMonsterMovement MonsterMovement = other.gameObject.GetComponent<BasicMonsterMovement>();
            MonsterMovement.TakeDamage(Damage, IsCrit);
            if (!CanHitMany)
            {
                IsAttackDone = true;
            }
        }
    }

    void FlipSprite()
    {
        if (XSpeed < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    void CheckMonster()
    {
        Vector2 Origin = Player.transform.position;
        Vector2 Direction = PlayerMovement.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 Size = new Vector2(BoxWidth, BoxHeight);
        float Angle = 0f;

        RaycastHit2D[] Hits = Physics2D.BoxCastAll(Origin, Size, Angle, Direction, AttackDistance, LayerMask.GetMask("Monster"));
        float MinDistance = float.MaxValue;
        GameObject ClosestMonster = null;

        foreach (RaycastHit2D Hit in Hits)
        {
            if (Hit.collider != null && Hit.collider.CompareTag("Monster"))
            {
                BasicMonsterMovement MonsterMovement = Hit.collider.GetComponent<BasicMonsterMovement>();
                if (MonsterMovement != null && MonsterMovement.IsAlive)
                {
                    float Distance = Vector2.Distance(Origin, Hit.point);
                    if (Distance < MinDistance)
                    {
                        MinDistance = Distance;
                        ClosestMonster = Hit.collider.gameObject;
                    }
                }
            }
        }

        if (ClosestMonster != null)
        {
            TargetMonster = ClosestMonster;
        }
        else
        {
            // 타겟 몬스터가 없으면 TargetPosition을 플레이어 앞쪽으로 설정
            TargetPosition = Player.transform.position + new Vector3(Direction.x * AttackDistance, 0, 0);
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}