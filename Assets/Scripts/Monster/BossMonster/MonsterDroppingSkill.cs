using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDroppingSkill : MonoBehaviour
{
    Animator MyAnimator;
    CapsuleCollider2D MyCapsuleCollider;
    BasicMonsterMovement BasicMonsterMovement;
    GameObject DropObjectInstance;
    [SerializeField] GameObject DropObject;
    [SerializeField] Transform SkillSpot;
    [SerializeField] float BackToIdleAnimTime = 0.35f;
    [SerializeField] float SkillDelayForAnim = 0f; // 애니메이션 재생 후 스킬 발사 대기시간
    public float UseSkillDistance = 60f; // 스킬 사용 거리

    void Start() {
        MyAnimator = GetComponent<Animator>();
        MyCapsuleCollider = GetComponent<CapsuleCollider2D>();
        BasicMonsterMovement = GetComponent<BasicMonsterMovement>();
    }
    public void ShootSkill() {
        bool IsOnGround = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        bool IsOnLadderGround = MyCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("LadderGround"));

        if (IsOnLadderGround || IsOnGround) {
            Invoke("InstantiateSkill", SkillDelayForAnim);
            MyAnimator.SetBool("IsDropping", true);
            BasicMonsterMovement.IsSkilling = true;
            BasicMonsterMovement.CanWalk = false;
            Invoke("BackToIdleAnim", BackToIdleAnimTime); // 일정 시간 이후 BackToIdleAnim 함수를 호출하여 Idle 애니메이션으로 변경
        }
    }

    void BackToIdleAnim() {
        MyAnimator.SetBool("IsDropping", false);
        BasicMonsterMovement.IsSkilling = false;
        BasicMonsterMovement.CanWalk = true;
    }

    void InstantiateSkill() {
        DropObjectInstance = Instantiate(DropObject, SkillSpot.position, transform.rotation);
        // MonsterAttackSkill 컴포넌트를 가져와 필요한 값을 할당
        MonsterAttackSkill MonsterAttackSkill = DropObjectInstance.GetComponent<MonsterAttackSkill>();
        if (MonsterAttackSkill != null) {
            BasicMonsterMovement BasicMonsterMovement = GetComponent<BasicMonsterMovement>();
            MonsterStatus MonsterStatus = GetComponent<MonsterStatus>();
            MonsterAttackSkill.BasicMonsterMovement = BasicMonsterMovement;
            MonsterAttackSkill.MonsterStatus = MonsterStatus;
            MonsterAttackSkill.IsLeft = BasicMonsterMovement.IsLeft;
        }
    }
}
