using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestToolTip : MonoBehaviour
{
    [SerializeField] GameObject QuestToolTipBase;
    [SerializeField] TMP_Text QuestNameText;
    [SerializeField] TMP_Text QuestNPCText;
    [SerializeField] TMP_Text QuestPlaceText;
    [SerializeField] TMP_Text QuestDescText;
    [SerializeField] TMP_Text QuestProgressText;

    public void ShowToolTip(Quest quest)
    {
        QuestToolTipBase.SetActive(true);
        if (quest == null)
        {
            return;
        }

        QuestNameText.text = quest.Title;
        QuestDescText.text = quest.QuestDescription;
        QuestNPCText.text = "NPC 이름: " + quest.NPCName;
        QuestPlaceText.text = "위치: " + quest.Place;
        // 목표 진행 상황 초기화
        QuestProgressText.text = "";

        // 각 목표의 진행 상황을 텍스트에 추가
        foreach (var objective in quest.Objectives)
        {
            // 퀘스트가 완료된 상태라면 CurrentAmount를 RequiredAmount와 동일하게 설정
            if (quest.IsCompleted)
            {
                objective.CurrentAmount = objective.RequiredAmount;
            }
            if (quest.Objectives.TrueForAll(obj => obj.Type == QuestObjective.ObjectiveType.None)) {
                QuestProgressText.text = quest.NPCName + "과 대화하기";
            }
            else {
            QuestProgressText.text += $"{objective.TargetName}: {objective.CurrentAmount}/{objective.RequiredAmount}\n";
            }
        }
    }

    public void HideToolTip()
    {
        QuestToolTipBase.SetActive(false);
    }
}
