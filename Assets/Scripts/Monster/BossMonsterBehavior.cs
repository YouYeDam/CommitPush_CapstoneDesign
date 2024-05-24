using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterBehavior : MonoBehaviour
{
    [SerializeField] bool IsSummonBoss = false; // 소환스킬을 가진 보스인지
    [SerializeField] bool IsThrowingBoss = false; // 던지기 스킬을 가진 보스인지
    [SerializeField] float SummonWaitTime = 20f;
    [SerializeField] float ThrowingWaitTime = 10f;
    [SerializeField] float SummonBackToIdleAnimTime = 0.35f;
    [SerializeField] float SummonDelayTime = 1f;
    [SerializeField] int SummonCount = 0;
    public Transform SummonSpot;
    BasicMonsterMovement BasicMonsterMovement;
    GenerateMonster GenerateMonster;
    MonsterThrowingSkill MonsterThrowingSkill;
    MonsterSkills MonsterSkills;
    Animator MyAnimator;
    PlayerMovement PlayerMovement;

    void Start()
    {
        BasicMonsterMovement = GetComponent<BasicMonsterMovement>();
        MonsterSkills = GetComponent<MonsterSkills>();
        MyAnimator = GetComponent<Animator>();
        PlayerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();


        if (IsSummonBoss) {
            GenerateMonster = GetComponent<GenerateMonster>();
            StartCoroutine(SummonMonster());
        }

        if (IsThrowingBoss) {
            MonsterThrowingSkill = GetComponent<MonsterThrowingSkill>();
            StartCoroutine(ThrowingSkill());
        }
    }

    IEnumerator SummonMonster() {
        while (BasicMonsterMovement.IsAlive && PlayerMovement.IsAlive && SummonCount <= 30) {
            yield return new WaitForSeconds(SummonWaitTime);

            if (MonsterSkills.IsSkilling) {
                yield return new WaitForSeconds(5f); // 스킬을 사용하는 중이면 추가 대기 시간
            }
            MyAnimator.SetBool("IsSummoning", true);
            BasicMonsterMovement.CanWalk = false;
            MonsterSkills.IsSkilling = true;

            yield return new WaitForSeconds(SummonBackToIdleAnimTime);
            MyAnimator.SetBool("IsSummoning", false);
            BasicMonsterMovement.CanWalk = true;
            MonsterSkills.IsSkilling = false;

            yield return new WaitForSeconds(SummonDelayTime);
            GenerateMonster.GenerateMonsters(SummonSpot.position);
            SummonCount++;
        }
    }

    IEnumerator ThrowingSkill() {
        while (BasicMonsterMovement.IsAlive && PlayerMovement.IsAlive) {
            yield return new WaitForSeconds(ThrowingWaitTime);

            if (MonsterSkills.IsSkilling) {
                yield return new WaitForSeconds(5f); // 스킬을 사용하는 중이면 추가 대기 시간
            }
            MonsterThrowingSkill.ShootSkill();
        }
    }
}
