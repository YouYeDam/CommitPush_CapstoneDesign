using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRegenerationController : MonoBehaviour
{
    [SerializeField] float DelayTime = 10f; // 몬스터 재생성까지 걸리는 시간
    [SerializeField] float ColliderDisableDelay = 1f; // 몬스터에게 boxcollider가 생길때 까지의 시간
    BasicMonsterMovement BasicMonsterMovement;
    public void RegenerateMonster(GameObject MonsterObject, Vector3 MonsterPosition, Quaternion MonsterRotation) { // 몬스터 생성 함수
        GameObject MonsterInstance = Instantiate(MonsterObject, MonsterPosition, MonsterRotation);

        BasicMonsterMovement = MonsterInstance.GetComponent<BasicMonsterMovement>();
        BasicMonsterMovement.IsTakeDamge = false;
        BasicMonsterMovement.CanWalk = true;
        BasicMonsterMovement.IsSkilling = false;
        MonsterInstance.SetActive(false);
        StartCoroutine(RegenerateMonsterWithDelay(MonsterInstance));
    }
    
    IEnumerator RegenerateMonsterWithDelay(GameObject MonsterInstance) {
        yield return new WaitForSeconds(DelayTime); // DelayTime만큼 대기
        MonsterInstance.SetActive(true);
        BasicMonsterMovement.CanTriggerDamage = false;

        yield return new WaitForSeconds(ColliderDisableDelay);
        BasicMonsterMovement.CanTriggerDamage = true;
    }
}