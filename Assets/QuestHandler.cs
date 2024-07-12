using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestHandler : MonoBehaviour
{
    [SerializeField] private WindowQuestPointer windowQuestPointer;

    [SerializeField] TMP_Text questTextDisplay;
    
    public void SetQuestTarget(Vector3 targetPos, string questText)
    {
        windowQuestPointer.Show(targetPos);
        questTextDisplay.text = questText;
    }

    public void QuestCompleted()
    {
        windowQuestPointer.Hide();
    }
}
