using TMPro;
using UnityEngine;
public class PlayerStatus : MonoBehaviour
{
    public string PlayerName; // 플레이어 이름
    public string PlayerClass = "개발자";
    public GameObject UIManager;
    public GameObject PlayerNameInfo; // 플레이어 이름 텍스트 프리팹
    public TMP_Text PlayerNameInfoText;
    public GameObject PlayerNameInfoInstance;
    public float PlayerNameInfoPos = 0.5f;
    [SerializeField] public int PlayerLevel = 1;
    [SerializeField] public int PlayerMaxHP = 100;
    [SerializeField] public int PlayerCurrentHP;
    [SerializeField] public int PlayerMaxMP = 100;
    [SerializeField] public int PlayerCurrentMP;
    [SerializeField] public int PlayerMaxEXP = 10;
    [SerializeField] public int PlayerCurrentEXP = 0;
    [SerializeField] public int PlayerATK = 1;  //공격력
    [SerializeField] public int PlayerDEF = 0; //방어력
    [SerializeField] public float PlayerAP = 0f; //가속력
    [SerializeField] public float PlayerCrit = 0f; //치명타율

    void Start() {
        PlayerCurrentHP = PlayerMaxHP;
        PlayerCurrentMP = PlayerMaxMP;
        UIManager = GameObject.Find("UIManager");
    }
    void Update() {
        UpdatePlayerNameInfo();
    }
    public void SetPlayerName(string NewName){
            PlayerName = NewName;
    }

    public void GainEXP(int EXP) {
        PlayerCurrentEXP += EXP;
        if (PlayerCurrentEXP >= PlayerMaxEXP) {
            LevelUp();
            PlayerMaxEXP = PlayerMaxEXP + (int)Mathf.Floor(PlayerMaxEXP * 0.5f);
        }
    }

    void LevelUp() {
        PlayerLevel += 1;
        PlayerCurrentEXP -= PlayerMaxEXP;
        PlayerCurrentHP = PlayerMaxHP;
        PlayerCurrentMP = PlayerMaxMP;
    }

    public void DisplayPlayerNameInfo() { // 캐릭터 이름 보이기
        if (UIManager != null && PlayerNameInfo != null && PlayerNameInfoInstance == null) {
            PlayerNameInfoInstance = Instantiate(PlayerNameInfo, UIManager.transform); // 캔버스의 자식으로 할당
            PlayerNameInfoText = PlayerNameInfoInstance.GetComponent<TMP_Text>();
            PlayerNameInfoText.text = PlayerName;
        }
    }

    void UpdatePlayerNameInfo() {
        if (PlayerNameInfoInstance != null) {
            Vector3 newPosition = transform.position + Vector3.down * PlayerNameInfoPos;
            PlayerNameInfoInstance.transform.position = newPosition;
        }
    }
}
