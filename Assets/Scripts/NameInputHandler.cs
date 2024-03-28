using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInputHandler : MonoBehaviour
{
    public TMP_InputField InputField; // Inspector에서 할당
    public Button SubmitButton; // Inspector에서 할당
    public PlayerStatus PlayerName; // Inspector에서 할당

    public void SubmitName()
    {
        // InputField에서 텍스트를 가져와 PlayerStatus 스크립트의 PlayerName에 할당
        PlayerName.SetPlayerName(InputField.text);
    }
}
