using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkills : MonoBehaviour
{
    Animator MyAnimator;
    CapsuleCollider2D MyCapsuleCollider;
    BoxCollider2D[] MyBoxColliders;
    PlayerMovement PlayerMovement;
    PlayerManager PlayerManager;
    PlayerStatus PlayerStatus;
    [SerializeField] GameObject NormalAttack;
    public GameObject QSkill;
    public GameObject WSkill;
    public GameObject ESkill;
    public GameObject RSkill;
    public GameObject SSkill;
    public GameObject DSkill;

    [SerializeField] Transform SkillSpot;
    [SerializeField] Transform BuffSpot;
    [SerializeField] float GlobalCoolDown = 0.3f;
    [SerializeField] float QSkillCoolDown;
    [SerializeField] float WSkillCoolDown;
    [SerializeField] float ESkillCoolDown;
    [SerializeField] float RSkillCoolDown;
    [SerializeField] float SSkillCoolDown;
    [SerializeField] float DSkillCoolDown;

    [SerializeField] float BackToIdleAnimTime = 0.2f;

    bool CanAttack = true;
    bool CanQSkill = true;
    bool CanWSkill = true;
    bool CanESkill = true;
    bool CanRSkill = true;
    bool CanSSkill = true;
    bool CanDSkill = true;

    int QSkillMPUse;
    int WSkillMPUse;
    int ESkillMPUse;
    int RSkillMPUse;
    int SSkillMPUse;
    int DSkillMPUse;

    void Start() {
        MyAnimator = GetComponent<Animator>();
        MyCapsuleCollider = GetComponent<CapsuleCollider2D>();
        MyBoxColliders = GetComponents<BoxCollider2D>();
        PlayerMovement = FindObjectOfType<PlayerMovement>();
        PlayerStatus = FindObjectOfType<PlayerStatus>();
        PlayerManager = GetComponent<PlayerManager>();
        GlobalCoolDown -= PlayerStatus.PlayerAP; // 글쿨 가속력 공식: 글쿨 - 가속력
    }

    void OnNormalAttack() {
        if (PlayerMovement.IsAlive == false || !PlayerManager.CanInput) {
            return;
        }
        bool IsOnLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        bool IsOnLadderGround = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("LadderGround"));
        bool IsSteppingLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")) && !MyBoxColliders[1].IsTouchingLayers(LayerMask.GetMask("Ladder"));
        if (CanAttack) {
            if (!IsOnLadder || IsOnLadderGround || IsSteppingLadder) {
                Instantiate(NormalAttack, SkillSpot.position, transform.rotation);
                MyAnimator.SetBool("IsAttacking", true);
                CanAttack = false; // 스킬을 사용한 후 플래그를 false로 설정
                PlayerMovement.IsWalkingAllowed = false; // 스킬을 사용한 후 이동 멈춤 설정
                
                Invoke("ResetCanAttack", GlobalCoolDown); // 쿨다운 이후 ResetSkill 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("BackToIdleAnim", BackToIdleAnimTime); // 일정 시간 이후 BackToIdleAnim 함수를 호출하여 Idle 애니메이션으로 변경
            }
        }
    }

    void OnQSkill() {
        if (PlayerMovement.IsAlive == false || !PlayerManager.CanInput || QSkill == null || PlayerStatus.PlayerCurrentMP < QSkillMPUse) {
            return;
        }
        bool IsOnLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        bool IsOnLadderGround = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("LadderGround"));
        bool IsSteppingLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")) && !MyBoxColliders[1].IsTouchingLayers(LayerMask.GetMask("Ladder"));
        if (CanAttack && CanQSkill) {
            if (!IsOnLadder || IsOnLadderGround || IsSteppingLadder) {
                if (QSkill.GetComponent<SkillInfo>().SkillType == "공격형") {
                    Instantiate(QSkill, SkillSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsAttacking", true);
                }
                else {
                    Instantiate(QSkill, BuffSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsBuffing", true);
                }
                CanAttack = false; // 스킬을 사용한 후 플래그를 false로 설정
                CanQSkill = false;
                PlayerMovement.IsWalkingAllowed = false; // 스킬을 사용한 후 이동 멈춤 설정

                PlayerStatus.PlayerCurrentMP -= QSkillMPUse;
                if (PlayerStatus.PlayerCurrentMP < 0) {
                    PlayerStatus.PlayerCurrentMP = 0;
                }

                Invoke("ResetCanAttack", GlobalCoolDown); // 쿨다운 이후 ResetCanAttack 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("ResetQSkill", QSkillCoolDown); // 쿨다운 이후 RestQSkill 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("BackToIdleAnim", BackToIdleAnimTime); // 일정 시간 이후 BackToIdleAnim 함수를 호출하여 Idle 애니메이션으로 변경
            }
        }
    }

    void OnWSkill() {
        if (PlayerMovement.IsAlive == false || !PlayerManager.CanInput || WSkill == null || PlayerStatus.PlayerCurrentMP < WSkillMPUse) {
            return;
        }
        bool IsOnLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        bool IsOnLadderGround = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("LadderGround"));
        bool IsSteppingLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")) && !MyBoxColliders[1].IsTouchingLayers(LayerMask.GetMask("Ladder"));
        if (CanAttack && CanWSkill) {
            if (!IsOnLadder || IsOnLadderGround || IsSteppingLadder) {
                if (WSkill.GetComponent<SkillInfo>().SkillType == "공격형") {
                    Instantiate(WSkill, SkillSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsAttacking", true);
                }
                else {
                    Instantiate(WSkill, BuffSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsBuffing", true);
                }
                CanAttack = false; // 스킬을 사용한 후 플래그를 false로 설정
                CanWSkill = false;
                PlayerMovement.IsWalkingAllowed = false; // 스킬을 사용한 후 이동 멈춤 설정

                PlayerStatus.PlayerCurrentMP -= WSkillMPUse;
                if (PlayerStatus.PlayerCurrentMP < 0) {
                    PlayerStatus.PlayerCurrentMP = 0;
                }

                Invoke("ResetCanAttack", GlobalCoolDown); // 쿨다운 이후 ResetCanAttack 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("ResetWSkill", WSkillCoolDown); // 쿨다운 이후 RestQSkill 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("BackToIdleAnim", BackToIdleAnimTime); // 일정 시간 이후 BackToIdleAnim 함수를 호출하여 Idle 애니메이션으로 변경
            }
        }
    }
    void OnESkill() {
        if (PlayerMovement.IsAlive == false || !PlayerManager.CanInput || ESkill == null || PlayerStatus.PlayerCurrentMP < ESkillMPUse) {
            return;
        }
        bool IsOnLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        bool IsOnLadderGround = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("LadderGround"));
        bool IsSteppingLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")) && !MyBoxColliders[1].IsTouchingLayers(LayerMask.GetMask("Ladder"));
        if (CanAttack && CanESkill) {
            if (!IsOnLadder || IsOnLadderGround || IsSteppingLadder) {
                if (ESkill.GetComponent<SkillInfo>().SkillType == "공격형") {
                    Instantiate(ESkill, SkillSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsAttacking", true);
                }
                else {
                    Instantiate(ESkill, BuffSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsBuffing", true);
                }
                CanAttack = false; // 스킬을 사용한 후 플래그를 false로 설정
                CanESkill = false;
                PlayerMovement.IsWalkingAllowed = false; // 스킬을 사용한 후 이동 멈춤 설정

                PlayerStatus.PlayerCurrentMP -= ESkillMPUse;
                if (PlayerStatus.PlayerCurrentMP < 0) {
                    PlayerStatus.PlayerCurrentMP = 0;
                }

                Invoke("ResetCanAttack", GlobalCoolDown); // 쿨다운 이후 ResetCanAttack 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("ResetESkill", ESkillCoolDown); // 쿨다운 이후 RestQSkill 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("BackToIdleAnim", BackToIdleAnimTime); // 일정 시간 이후 BackToIdleAnim 함수를 호출하여 Idle 애니메이션으로 변경
            }
        }
    }
    void OnRSkill() {
        if (PlayerMovement.IsAlive == false || !PlayerManager.CanInput || RSkill == null || PlayerStatus.PlayerCurrentMP < RSkillMPUse) {
            return;
        }
        bool IsOnLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        bool IsOnLadderGround = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("LadderGround"));
        bool IsSteppingLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")) && !MyBoxColliders[1].IsTouchingLayers(LayerMask.GetMask("Ladder"));
        if (CanAttack && CanRSkill) {
            if (!IsOnLadder || IsOnLadderGround || IsSteppingLadder) {
                if (RSkill.GetComponent<SkillInfo>().SkillType == "공격형") {
                    Instantiate(RSkill, SkillSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsAttacking", true);
                }
                else {
                    Instantiate(RSkill, BuffSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsBuffing", true);
                }
                CanAttack = false; // 스킬을 사용한 후 플래그를 false로 설정
                CanRSkill = false;
                PlayerMovement.IsWalkingAllowed = false; // 스킬을 사용한 후 이동 멈춤 설정

                PlayerStatus.PlayerCurrentMP -= RSkillMPUse;
                if (PlayerStatus.PlayerCurrentMP < 0) {
                    PlayerStatus.PlayerCurrentMP = 0;
                }

                Invoke("ResetCanAttack", GlobalCoolDown); // 쿨다운 이후 ResetCanAttack 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("ResetRSkill", RSkillCoolDown); // 쿨다운 이후 ResetRSkill 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("BackToIdleAnim", BackToIdleAnimTime); // 일정 시간 이후 BackToIdleAnim 함수를 호출하여 Idle 애니메이션으로 변경
            }
        }
    }

    void OnSSkill() {
        if (PlayerMovement.IsAlive == false || !PlayerManager.CanInput || SSkill == null || PlayerStatus.PlayerCurrentMP < SSkillMPUse) {
            return;
        }
        bool IsOnLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        bool IsOnLadderGround = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("LadderGround"));
        bool IsSteppingLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")) && !MyBoxColliders[1].IsTouchingLayers(LayerMask.GetMask("Ladder"));
        if (CanAttack && CanSSkill) {
            if (!IsOnLadder || IsOnLadderGround || IsSteppingLadder) {
                if (SSkill.GetComponent<SkillInfo>().SkillType == "공격형") {
                    Instantiate(SSkill, SkillSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsAttacking", true);
                }
                else {
                    Instantiate(SSkill, BuffSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsBuffing", true);
                }
                CanAttack = false; // 스킬을 사용한 후 플래그를 false로 설정
                CanSSkill = false;
                PlayerMovement.IsWalkingAllowed = false; // 스킬을 사용한 후 이동 멈춤 설정

                PlayerStatus.PlayerCurrentMP -= SSkillMPUse;
                if (PlayerStatus.PlayerCurrentMP < 0) {
                    PlayerStatus.PlayerCurrentMP = 0;
                }

                Invoke("ResetCanAttack", GlobalCoolDown); // 쿨다운 이후 ResetCanAttack 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("ResetSSkill", SSkillCoolDown); // 쿨다운 이후 ResetSSkill 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("BackToIdleAnim", BackToIdleAnimTime); // 일정 시간 이후 BackToIdleAnim 함수를 호출하여 Idle 애니메이션으로 변경
            }
        }
    }
    void OnDSkill() {
        if (PlayerMovement.IsAlive == false || !PlayerManager.CanInput || DSkill == null || PlayerStatus.PlayerCurrentMP < DSkillMPUse) {
            return;
        }
        bool IsOnLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        bool IsOnLadderGround = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("LadderGround"));
        bool IsSteppingLadder = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")) && !MyBoxColliders[1].IsTouchingLayers(LayerMask.GetMask("Ladder"));
        if (CanAttack && CanDSkill) {
            if (!IsOnLadder || IsOnLadderGround || IsSteppingLadder) {
                if (DSkill.GetComponent<SkillInfo>().SkillType == "공격형") {
                    Instantiate(DSkill, SkillSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsAttacking", true);
                }
                else {
                    Instantiate(DSkill, BuffSpot.position, transform.rotation);
                    MyAnimator.SetBool("IsBuffing", true);
                }
                CanAttack = false; // 스킬을 사용한 후 플래그를 false로 설정
                CanDSkill = false;
                PlayerMovement.IsWalkingAllowed = false; // 스킬을 사용한 후 이동 멈춤 설정

                PlayerStatus.PlayerCurrentMP -= DSkillMPUse;
                if (PlayerStatus.PlayerCurrentMP < 0) {
                    PlayerStatus.PlayerCurrentMP = 0;
                }

                Invoke("ResetCanAttack", GlobalCoolDown); // 쿨다운 이후 ResetCanAttack 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("ResetDSkill", DSkillCoolDown); // 쿨다운 이후 ResetDSkill 함수를 호출하여 스킬 사용 가능 상태로 변경
                Invoke("BackToIdleAnim", BackToIdleAnimTime); // 일정 시간 이후 BackToIdleAnim 함수를 호출하여 Idle 애니메이션으로 변경
            }
        }
    }

    void ResetCanAttack() {
        CanAttack = true;
    }

    void ResetQSkill() {
        CanQSkill = true;
    }
    void ResetWSkill() {
        CanWSkill = true;
    }
    void ResetESkill() {
        CanESkill = true;
    }
    void ResetRSkill() {
        CanRSkill = true;
    }
    void ResetSSkill() {
        CanSSkill = true;
    }
    void ResetDSkill() {
        CanDSkill = true;
    }
    void BackToIdleAnim() {
        MyAnimator.SetBool("IsAttacking", false);
        MyAnimator.SetBool("IsBuffing", false);
    }

    public void SetSkillsCoolTime(string ButtonKey) {
        switch (ButtonKey) {
            case "Q":
                PlayerAttackSkill PlayerQAttackSkill = QSkill.GetComponent<PlayerAttackSkill>();
                PlayerBuffSkill PlayerQBuffSkill = QSkill.GetComponent<PlayerBuffSkill>();

                if (PlayerQAttackSkill != null) {
                    QSkillCoolDown = PlayerQAttackSkill.CoolDown;
                    QSkillMPUse = PlayerQAttackSkill.MPUse;
                    }
            else if (PlayerQBuffSkill != null) {
                    QSkillCoolDown = PlayerQBuffSkill.CoolDown;
                    QSkillMPUse = PlayerQBuffSkill.MPUse;
                    }
                break;
            case "W":
                PlayerAttackSkill PlayerWAttackSkill = WSkill.GetComponent<PlayerAttackSkill>();
                PlayerBuffSkill PlayerWBuffSkill = WSkill.GetComponent<PlayerBuffSkill>();

                if (PlayerWAttackSkill != null) {
                    WSkillCoolDown = PlayerWAttackSkill.CoolDown;
                    WSkillMPUse = PlayerWAttackSkill.MPUse;
                }
                else if (PlayerWBuffSkill != null) {
                    WSkillCoolDown = PlayerWBuffSkill.CoolDown;
                    WSkillMPUse = PlayerWBuffSkill.MPUse;
                }
                break;

            case "E":
                PlayerAttackSkill PlayerEAttackSkill = ESkill.GetComponent<PlayerAttackSkill>();
                PlayerBuffSkill PlayerEBuffSkill = ESkill.GetComponent<PlayerBuffSkill>();

                if (PlayerEAttackSkill != null) {
                    ESkillCoolDown = PlayerEAttackSkill.CoolDown;
                    ESkillMPUse = PlayerEAttackSkill.MPUse;
                }
                else if (PlayerEBuffSkill != null) {
                    ESkillCoolDown = PlayerEBuffSkill.CoolDown;
                    ESkillMPUse = PlayerEBuffSkill.MPUse;
                }
                break;
            case "R":
                PlayerAttackSkill PlayerRAttackSkill = RSkill.GetComponent<PlayerAttackSkill>();
                PlayerBuffSkill PlayerRBuffSkill = RSkill.GetComponent<PlayerBuffSkill>();

                if (PlayerRAttackSkill != null) {
                    RSkillCoolDown = PlayerRAttackSkill.CoolDown;
                    RSkillMPUse = PlayerRAttackSkill.MPUse;
                }
                else if (PlayerRBuffSkill != null) {
                    RSkillCoolDown = PlayerRBuffSkill.CoolDown;
                    RSkillMPUse = PlayerRBuffSkill.MPUse;
                }
                break;
            case "S":
                PlayerAttackSkill PlayerSAttackSkill = SSkill.GetComponent<PlayerAttackSkill>();
                PlayerBuffSkill PlayerSBuffSkill = SSkill.GetComponent<PlayerBuffSkill>();

                if (PlayerSAttackSkill != null) {
                    SSkillCoolDown = PlayerSAttackSkill.CoolDown;
                    SSkillMPUse = PlayerSAttackSkill.MPUse;
                }
                else if (PlayerSBuffSkill != null) {
                    SSkillCoolDown = PlayerSBuffSkill.CoolDown;
                    SSkillMPUse = PlayerSBuffSkill.MPUse;
                }
                break;
            case "D":
                PlayerAttackSkill PlayerDAttackSkill = DSkill.GetComponent<PlayerAttackSkill>();
                PlayerBuffSkill PlayerDBuffSkill = DSkill.GetComponent<PlayerBuffSkill>();

                if (PlayerDAttackSkill != null) {
                    DSkillCoolDown = PlayerDAttackSkill.CoolDown;
                    DSkillMPUse = PlayerDAttackSkill.MPUse;
                }
                else if (PlayerDBuffSkill != null) {
                    DSkillCoolDown = PlayerDBuffSkill.CoolDown;
                    DSkillMPUse = PlayerDBuffSkill.MPUse;
                }
                break;
            default:
                break;
        }
    }
}

