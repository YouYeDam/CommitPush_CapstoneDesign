using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using UnityEditor;

[System.Serializable]
public class PlayerData
{
    public float x, y, z; // 플레이어 위치
    public int PlayerLevel;
    public string PlayerName;
    public string PlayerClass;
    public GameObject PlayerNameInfo; // 플레이어 이름 텍스트 프리팹
    public TMP_Text PlayerNameInfoText;
    public GameObject PlayerNameInfoInstance;
    public int PlayerCurrentHP;
    public int PlayerMaxHP;
    public int PlayerCurrentMP;
    public int PlayerMaxMP;
    public int PlayerCurrentEXP;
    public int PlayerMaxEXP;
    public int PlayerATK;
    public int PlayerDEF;
    public float PlayerAP;
    public float PlayerCrit;
    public int LevelUpPoint;
    public int Bit;
    public int Snippet;
    public string sceneName; // 현재 씬 이름
    public GameObject playerPrefab;
    public Animator playerAnimator; // 이거 필요 없었던 거 같은데
    public RuntimeAnimatorController runtimeAnimatorController;
    // 아이템 저장을 위해선 item 과 item count로 구성된 2차원 배열을 저장해야함.
    public string[] itemNames;
    public string[] skillNames = new string[10];
    public string[] quickSkillNames = new string[6];
    public int[] skillSlotReferencesIdx;
    public int[] itemSlotReferencesIdx;
    public string[] quickItemNames;
    public int[] quickItemCounts;
    public int[] quickItemCosts;
    public int[] itemCounts;
    public int[] itemCosts;
    public GameObject QSkill, WSkill, ESkill, RSkill, SSkill, DSkill;
    public float QSkillCoolDown, WSkillCoolDown, ESkillCoolDown, RSkillCoolDown, SSkillCoolDown, DSkillCoolDown;
    // quest datas. 필요 없는거 나중에 제거
    public List<QuestData> QuestDatas; // 모든 퀘스트 데이터 리스트
    public List<Quest> allQuests = new List<Quest>(); // 모든 퀘스트 리스트
    public List<Quest> activeQuests = new List<Quest>(); // 활성화된 퀘스트 리스트
    public List<Quest> completedQuests = new List<Quest>(); // 완료된 퀘스트 리스트
    public Dictionary<string, int> NPCQuestIndices = new Dictionary<string, int>(); // 완료된 퀘스트 리스트
    public Quest[] quests = new Quest[30];
    public Dictionary<string, int> currentQuestIndex = new Dictionary<string, int>();
    public string runtimeAnimatorControllerPath;
    // 장비 저장 데이터
    public string[] equipmentNames;
}

public class SaveManager : MonoBehaviour
{
    public GameObject playerPrefab; // 플레이어 프리팹 참조
    public GameObject uiManagerPrefab; // UIManager 프리팹 참조
    public GameObject questManagerPrefab; // UIManager 프리팹 참조
    public GameObject PaueseMenu; // Pause Menu 프리팹 참조
    private GameObject PauseMenuInstance = null;
    public GameObject playerObject;
    public GameObject questManagerObject;
    QuestManager questManager;
    UIManager uiManager;
    PlayerStatus playerStatus;
    GameObject uiManagerObject;

    public SkillSlot[] skillSlots;
    public EquipmentSlot[] equipmentSlots;
    void Start()
    {
        playerObject = null;
        questManagerObject = null;
        uiManager = null;
        skillSlots = null;
        equipmentSlots = null;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name != "Main Menu Scene" && SceneManager.GetActiveScene().name != "Create Player Scene")
            {
                ActivatePauseMenu();
            }
        }
    }

    void DoSave()
    {
        playerObject = GameObject.FindWithTag("Player");
        questManagerObject = GameObject.FindWithTag("QuestManager");
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        if (playerObject != null)
        {
            SavePlayerProgress(SceneManager.GetActiveScene().name, uiManager, playerObject, questManagerObject);
        }
        InactivatePauseMenu();
    }

    public void SavePlayerProgress(string currentSceneName, UIManager uIManager, GameObject playerObject, GameObject questManagerObject)
    {
        Slot[] slots = uiManager.GetComponentsInChildren<Slot>(true); // 슬롯은 기본적으로 비활성화 돼있기 때문에 (true) 값을 설정. 모든 슬롯을 가져옴.
        EquipmentSlot[] equipmentSlots = uiManager.GetComponentsInChildren<EquipmentSlot>(true);
        SkillSlot[] skillSlots = uiManager.GetComponentsInChildren<SkillSlot>(true);
        PlayerStatus playerStatus = playerObject.GetComponent<PlayerStatus>();
        PlayerMoney playerMoney = playerObject.GetComponent<PlayerMoney>();
        Vector3 playerPosition = playerObject.transform.position;
        PlayerSkills playerSkills = playerObject.GetComponent<PlayerSkills>();
        SkillQuickSlot[] skillQuickSlots = uIManager.GetComponentsInChildren<SkillQuickSlot>();
        ItemQuickSlot[] itemQuickSlots = uIManager.GetComponentsInChildren<ItemQuickSlot>();
        QuestManager questManager = questManagerObject.GetComponent<QuestManager>();
        QuestSlot[] questSlots = uiManager.gameObject.GetComponentsInChildren<QuestSlot>(true);
        // Save
        string animatorControllerName = playerObject.GetComponent<Animator>().runtimeAnimatorController.name;


        PlayerData data = new PlayerData
        {
            x = playerPosition.x,
            y = playerPosition.y,
            z = playerPosition.z,
            sceneName = currentSceneName, // 현재 씬 이름 저장
            PlayerLevel = playerStatus.PlayerLevel,
            PlayerMaxHP = playerStatus.PlayerMaxHP,
            PlayerCurrentHP = playerStatus.PlayerCurrentHP, // 플레이어 체력 저장
            PlayerMaxMP = playerStatus.PlayerMaxMP,
            PlayerCurrentMP = playerStatus.PlayerCurrentMP,
            PlayerMaxEXP = playerStatus.PlayerMaxEXP,
            PlayerCurrentEXP = playerStatus.PlayerCurrentEXP,
            PlayerName = playerStatus.PlayerName,
            PlayerClass = playerStatus.PlayerClass,
            PlayerNameInfo = playerStatus.PlayerNameInfo,
            PlayerNameInfoText = playerStatus.PlayerNameInfoText,
            PlayerNameInfoInstance = playerStatus.PlayerNameInfoInstance,
            PlayerATK = playerStatus.PlayerATK,
            PlayerDEF = playerStatus.PlayerDEF,
            PlayerAP = playerStatus.PlayerAP,
            PlayerCrit = playerStatus.PlayerCrit,
            LevelUpPoint = playerStatus.LevelUpPoint,
            Bit = playerMoney.Bit,
            Snippet = playerMoney.Snippet,
            QSkill = playerSkills.QSkill,
            WSkill = playerSkills.WSkill,
            ESkill = playerSkills.ESkill,
            RSkill = playerSkills.RSkill,
            SSkill = playerSkills.SSkill,
            DSkill = playerSkills.DSkill,
            QSkillCoolDown = playerSkills.QSkillCoolDown,
            
            WSkillCoolDown = playerSkills.WSkillCoolDown,
            ESkillCoolDown = playerSkills.ESkillCoolDown,
            RSkillCoolDown = playerSkills.RSkillCoolDown,
            SSkillCoolDown = playerSkills.SSkillCoolDown,
            DSkillCoolDown = playerSkills.DSkillCoolDown,
            runtimeAnimatorControllerPath = "AnimatorControllers/" + animatorControllerName,
            //8888
            QuestDatas = questManager.QuestDatas,
            allQuests = questManager.allQuests,
            activeQuests = questManager.activeQuests,
            completedQuests = questManager.completedQuests,
        };
        Debug.Log("cool down saved: " + data.QSkillCoolDown );

        SaveItems(slots, data);
        SaveSkills(skillSlots, data);
        SaveQuickSkills(skillQuickSlots, data);
        SaveQuickItems(itemQuickSlots, data);
        SaveEquipments(equipmentSlots, data);
        SaveQuestSlot(questSlots, data, questManager);
        //  9999
        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "playerData.json");
        File.WriteAllText(path, json);
        PlayerPrefs.SetString("PlayerProgress", json);
        PlayerPrefs.Save();
    }

    private static void SaveQuestSlot(QuestSlot[] questSlots, PlayerData data, QuestManager questManager)
    {
        NPC[] npcs = FindObjectsOfType<NPC>();
        data.NPCQuestIndices = questManager.NpcQuestState.NpcQuestIndices;
        foreach (Quest quest in questManager.allQuests)
        {
            string questTitle = quest.Title;
            foreach (NPC npc in npcs)
            {
                for (int i = 0; i < npc.QuestsToGive.Count; i++)
                {
                    if (npc.QuestsToGive[i].Title == questTitle) // 모든 퀘스트 타이틀에 대해 이터레이션. 
                    {
                        data.currentQuestIndex[questTitle] = questManager.NpcQuestState.GetQuestIndex(npc.NPCName);
                    }
                }
            }
        }
        
        // quests in slots
        Quest[] quests = new Quest[questSlots.Length];
        for (int i = 0; i < questSlots.Length; i++)
        {
            if(questSlots[i].Quest.Title != "")
            {
                Quest quest2Save = questSlots[i].Quest;
                quests[i] = quest2Save;
                Debug.Log("Save: quest name " + quest2Save.Title);
                Debug.Log("Save: quest bools IsReadyToComplete " + quest2Save.IsReadyToComplete);
                Debug.Log("Save: quest bools  IsCompleted " + quest2Save.IsCompleted);
            }
        }
        data.quests = quests;
    }

    private static void SaveEquipments(EquipmentSlot[] equipmentSlots, PlayerData data)
    {
        string[] equipmentNames = new string[equipmentSlots.Length];

        // 각 슬롯의 equipment item을 저장
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].Item != null)
            {
                equipmentNames[i] = equipmentSlots[i].Item.name;
                Debug.Log("equipment saved " + equipmentSlots[i].Item.name);
            }

        };
        // 정리된 배열들을 data에 저장
        data.equipmentNames = equipmentNames;
    }
    private static void SaveQuickSkills(SkillQuickSlot[] skillQuickSlots, PlayerData data)
    {
        string[] quickSkillNames = new string[skillQuickSlots.Length];
        int[] quickSkillCoolTime = new int[skillQuickSlots.Length];
        int[] skillSlotReferencesIdx = new int[skillQuickSlots.Length];
        for (int i = 0; i < skillQuickSlots.Length; i++)
        {
            if (skillQuickSlots[i].SkillPrefab != null)
            {
                quickSkillNames[i] = skillQuickSlots[i].SkillPrefab.name;
                Debug.Log("quick skill saved: " + quickSkillNames[i]);
            }
            if (skillQuickSlots[i].SlotReference != null)
            {
                skillSlotReferencesIdx[i] = skillQuickSlots[i].SlotReference.transform.GetSiblingIndex();
            }
        };
        // 정리된 배열들을 data에 저장
        data.quickSkillNames = quickSkillNames;
        data.skillSlotReferencesIdx = skillSlotReferencesIdx;
    }

    private static void SaveSkills(SkillSlot[] skillSlots, PlayerData data)
    {
        string[] skillNames = new string[skillSlots.Length];

        // 각 슬롯의 skillprefab을 저장
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i].SkillPrefab != null)
            {
                skillNames[i] = skillSlots[i].SkillPrefab.name;
                Debug.Log("skill saved: " + skillNames[i]);
            }
        };
        // 정리된 배열들을 data에 저장
        data.skillNames = skillNames;

    }
    private static void SaveItems(Slot[] slots, PlayerData data)
    {
        string[] itemNames = new string[slots.Length];
        int[] itemCounts = new int[slots.Length];
        int[] itemCosts = new int[slots.Length];

        // 각 슬롯의 Item과 ItemCount를 배열에 저장합니다. 함수를 통해 넘겨받은 슬롯의 아이템들을 배열로 정리
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item != null)
            {
                itemNames[i] = slots[i].Item.name;
                itemCounts[i] = slots[i].ItemCount;
                itemCosts[i] = slots[i].Item.ItemCost;
            }
        };
        // 정리된 배열들을 data에 저장
        data.itemNames = itemNames;
        data.itemCounts = itemCounts;
        data.itemCosts = itemCosts;
    }
    private static void SaveQuickItems(ItemQuickSlot[] itemQuickSlots, PlayerData data)
    {
        string[] itemNames = new string[itemQuickSlots.Length];
        int[] itemCounts = new int[itemQuickSlots.Length];
        int[] itemCosts = new int[itemQuickSlots.Length];
        int[] referSlotsIdx = new int[itemQuickSlots.Length];
        for (int i = 0; i < itemQuickSlots.Length; i++)
        {
            if (itemQuickSlots[i].Item != null)
            {
                itemNames[i] = itemQuickSlots[i].Item.name;
                itemCounts[i] = itemQuickSlots[i].ItemCount;
                itemCosts[i] = itemQuickSlots[i].Item.ItemCost;
                if (itemQuickSlots[i].SlotReference != null)
                {
                    referSlotsIdx[i] = itemQuickSlots[i].SlotReference.transform.GetSiblingIndex();
                }
            }
        };
        // 정리된 배열들을 data에 저장
        data.quickItemNames = itemNames;
        data.quickItemCounts = itemCounts;
        data.quickItemCosts = itemCosts;
        data.itemSlotReferencesIdx = referSlotsIdx;
    }

    public void DisplaySaveDatas(PlayerData data)
    {
        if (data == null)
        {
            Console.WriteLine("No data to display.");
            return;
        }

        Console.WriteLine("Save Data:");
        for (int i = 0; i < data.equipmentNames.Length; i++)
        {
        }
        for (int i = 0; i < data.itemNames.Length; i++)
        {
        }
        for (int i = 0; i < data.quickItemNames.Length; i++)
        {
        }
    }
    public void InstantiateOnSavePoint(PlayerData data)
    {
        questManagerObject = Instantiate(questManagerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        questManager = FindObjectOfType<QuestManager>();
        questManagerObject.name = questManagerPrefab.name; // "(Clone)" 접미사 제거
        DontDestroyOnLoad(questManagerObject); // 씬 전환 시 파괴되지 않도록 설정
        questManager.QuestDatas = data.QuestDatas;
        questManager.allQuests = data.allQuests;
        questManager.activeQuests = data.activeQuests;
        questManager.completedQuests = data.completedQuests;

        DisplaySaveDatas(data);
        if (playerObject == null)
        {
            playerObject = Instantiate(playerPrefab, new Vector3(data.x, data.y, data.z), Quaternion.identity);
            playerObject.name = playerPrefab.name; // "(Clone)" 접미사 제거
            DontDestroyOnLoad(playerObject); // 씬 전환 시 파괴되지 않도록 설정

            playerStatus = playerObject.GetComponent<PlayerStatus>();
            // set level
            playerStatus.PlayerLevel = data.PlayerLevel;
            // set max values
            playerStatus.PlayerMaxHP = data.PlayerMaxHP;
            playerStatus.PlayerMaxMP = data.PlayerMaxMP;
            playerStatus.PlayerMaxEXP = data.PlayerMaxEXP;
            // set current values.
            playerStatus.PlayerCurrentMP = data.PlayerCurrentMP;
            playerStatus.PlayerCurrentHP = data.PlayerCurrentHP;
            playerStatus.PlayerCurrentEXP = data.PlayerCurrentEXP;

            playerStatus.PlayerClass = data.PlayerClass;
            playerStatus.PlayerName = data.PlayerName;
            playerStatus.PlayerATK = data.PlayerATK;
            playerStatus.PlayerDEF = data.PlayerDEF;
            playerStatus.PlayerAP = data.PlayerAP;
            playerStatus.PlayerCrit = data.PlayerCrit;
            playerStatus.LevelUpPoint = data.LevelUpPoint;
            playerStatus.IsLoaded = true;

            playerObject.GetComponent<PlayerMoney>().Bit = data.Bit;
            playerObject.GetComponent<PlayerMoney>().Snippet = data.Snippet;

            playerObject.GetComponent<PlayerSkills>().QSkill = data.QSkill;
            playerObject.GetComponent<PlayerSkills>().WSkill = data.WSkill;
            playerObject.GetComponent<PlayerSkills>().ESkill = data.ESkill;
            playerObject.GetComponent<PlayerSkills>().RSkill = data.RSkill;
            playerObject.GetComponent<PlayerSkills>().SSkill = data.SSkill;
            playerObject.GetComponent<PlayerSkills>().DSkill = data.DSkill;


            // Load animator controller
            string animatorControllerPath = data.runtimeAnimatorControllerPath;
            playerObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(animatorControllerPath);
        }


        if (uiManagerObject == null)
        {
            // LevelUpPoint
            uiManagerObject = Instantiate(uiManagerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            // 이 지점에서 초기화 시켜줘야 오류가 없음.
            playerObject.GetComponent<PlayerLevelUpController>().Character = uiManagerObject.GetComponentInChildren<Character>(true).gameObject;
            // name tag initialize
            if (uiManagerObject != null && data.PlayerNameInfo != null && data.PlayerNameInfoInstance == null)
            {
                playerStatus.PlayerNameInfoInstance = Instantiate(data.PlayerNameInfo, uiManagerObject.transform);
                playerStatus.PlayerNameInfoInstance.GetComponent<TMP_Text>().text = data.PlayerName;
            }
            uiManager = uiManagerObject.GetComponent<UIManager>();
            uiManagerObject.name = uiManagerPrefab.name; // "(Clone)" 접미사 제거
            DontDestroyOnLoad(uiManagerObject); // 씬 전환 시 파괴되지 않도록 설정


            InitialEquipmentItems();
            LoadEquipments(data);
            LoadItems(data);
            LoadQuickItems(data);

            LoadSkills(data);
            LoadQuickSkills(data, playerObject);
            ShopSlotInit();
            LoadQuests(data, questManager);




        }


    }

    private void InitialEquipmentItems()
    {
        EquipmentItem[] equipmentItems = uiManager.GetComponentsInChildren<EquipmentItem>();
        for (int i = 0; i < equipmentItems.Length; i++)
        {
            equipmentItems[i].PlayerStatus = playerStatus;
        }
    }

    private void ShopSlotInit()
    {
        ShopSlot[] shopSlot = uiManager.GetComponentsInChildren<ShopSlot>(true);
        for (int i = 0; i < shopSlot.Length; i++)
        {
            shopSlot[i].QuestManager = questManager;
        }
    }

    private void LoadSkills(PlayerData data)
    {
        var skillSlots = uiManagerObject.GetComponentsInChildren<SkillSlot>(true);
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (data.skillNames[i] != null)
            {
                string skillAssetPath = "SkillPrefabs/" + data.skillNames[i];
                Debug.Log("Loaded skill name: " + skillAssetPath);
                GameObject newSkillPrefab = (GameObject)Resources.Load(skillAssetPath);
                if (newSkillPrefab == null)
                {
                    continue;
                }
                skillSlots[i].AddSkill(newSkillPrefab);

            }
        }
    }

    private void LoadQuests(PlayerData data, QuestManager questManager)
    {
        // questManager.NpcQuestState = gameObject.AddComponent<NPCQuestState>(); // NPCQuestState 추가
        // questManager.InitializeQuests();
        // questManager.ResetAllQuestObjectives();
        // questManager.QuestContent = GameObject.Find("UIManager").transform.Find("Quest/Scroll View/Viewport/QuestContent").gameObject;
        // questManager.QuestSlots = questManager.QuestContent.GetComponentsInChildren<QuestSlot>();
        // questManager.PlayerGetItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGetItem>();
        // questManager.PlayerMoney = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoney>();
        // questManager.PlayerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        NPC[] npcs = FindObjectsOfType<NPC>();
        questManager.NpcQuestState.NpcQuestIndices = data.NPCQuestIndices;
        foreach (Quest quest in data.allQuests)
        {
            string questTitle = quest.Title;
            foreach (NPC npc in npcs)
            {
                for (int i = 0; i < npc.QuestsToGive.Count; i++)
                {
                    if (npc.QuestsToGive[i].Title == questTitle) // 모든 퀘스트 타이틀에 대해 이터레이션. 
                    {
                        questManager.NpcQuestState.SetQuestIndex(npc.NPCName, data.currentQuestIndex[questTitle]);
                    }
                }
            }
        }


        for (int i = 0; i < data.quests.Length; i++)
        {
            uiManagerObject.GetComponentsInChildren<QuestSlot>(true)[i].Quest = null;
            if (data.quests[i].Title != "")
            {
                uiManagerObject.GetComponentsInChildren<QuestSlot>(true)[i].AddQuest(data.quests[i]);
                Debug.Log("Load: quest bools name " + data.quests[i].Title);
                Debug.Log("Load: quest bools IsReadyToComplete " + data.quests[i].IsReadyToComplete);
                Debug.Log("Load: quest bools  IsCompleted" + data.quests[i].IsCompleted);
            }
        }

    }

    private void LoadQuickSkills(PlayerData data, GameObject playerObject)
    {
        var quickSkillSlots = uiManagerObject.GetComponentsInChildren<SkillQuickSlot>(true);
        for (int i = 0; i < quickSkillSlots.Length; i++)
        {
            quickSkillSlots[i].PlayerSkills = playerObject.GetComponent<PlayerSkills>();
            SkillSlot refer_slot = null;
            if (data.quickSkillNames[i] != null)
            {
                string skillAssetPath = "SkillPrefabs/" + data.quickSkillNames[i];
                Debug.Log("Loaded quick skill name: " + skillAssetPath);
                GameObject newSkillPrefab = (GameObject)Resources.Load(skillAssetPath);
                if (newSkillPrefab == null)
                {
                    continue;
                }
                refer_slot = uiManagerObject.GetComponentsInChildren<SkillSlot>(true)[data.skillSlotReferencesIdx[i]];
                quickSkillSlots[i].AddSkill(newSkillPrefab, refer_slot);
                Debug.Log("quick skill loaded: " + newSkillPrefab.name);
            }
        }
        playerObject.GetComponent<PlayerSkills>().QSkillCoolDown = data.QSkillCoolDown;
        playerObject.GetComponent<PlayerSkills>().WSkillCoolDown = data.WSkillCoolDown;
        playerObject.GetComponent<PlayerSkills>().ESkillCoolDown = data.ESkillCoolDown;
        playerObject.GetComponent<PlayerSkills>().RSkillCoolDown = data.RSkillCoolDown;
        playerObject.GetComponent<PlayerSkills>().SSkillCoolDown = data.SSkillCoolDown;
        playerObject.GetComponent<PlayerSkills>().DSkillCoolDown = data.DSkillCoolDown;
        Debug.Log("cool down loaded: " + playerObject.GetComponent<PlayerSkills>().QSkillCoolDown + " = "+ data.QSkillCoolDown );

    }

    private void LoadEquipments(PlayerData data)
    {
        for (int i = 0; i < data.equipmentNames.Length; i++)
        {
            var equipmentSlots = uiManagerObject.GetComponentsInChildren<EquipmentSlot>(true);
            if (i >= equipmentSlots.Length)
            {
                break;
            }

            equipmentSlots[i].Item = null;

            if (data.equipmentNames[i] != null)
            {
                Item newEquipmentItem = ScriptableObject.CreateInstance<Item>();
                EquipmentItem equipmentItem = new EquipmentItem();

                newEquipmentItem.name = data.equipmentNames[i];
                newEquipmentItem.ItemCount = data.itemCounts[i];
                string itemAssetPath = "Items/" + newEquipmentItem.name;
                Item newItemClass = (Item)Resources.Load(itemAssetPath, typeof(Item));
                if (newItemClass == null)
                {
                    continue;
                }
                newEquipmentItem.ItemName = newItemClass.ItemName;
                newEquipmentItem.ItemCost = newItemClass.ItemCost;
                newEquipmentItem.ItemDetailType = newItemClass.ItemDetailType;
                newEquipmentItem.ItemImage = newItemClass.ItemImage;
                newEquipmentItem.ItemPrefab = newItemClass.ItemPrefab;
                newEquipmentItem.IsAlreadyGet = true;
                newEquipmentItem.ItemInfo = newItemClass.ItemInfo;
                newEquipmentItem.Type = newItemClass.Type;

                equipmentSlots[i].AddItemOnLoad(newEquipmentItem);
            }
        }
    }

    private void LoadItems(PlayerData data) // data의 아이템이 아이템인지 장비인지 구별해야함. isEquipment 함수를 자료형에 추가하고. 또 장비의 슬롯 인덱스도 저장해야함.
    {
        // int[] tmpCnt = new int[data.itemNames.Length];
        var itemSlots = uiManagerObject.GetComponentsInChildren<Slot>(true);
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (data.itemNames[i] != null)
            {

                // Item 객체 생성
                Item newItem = ScriptableObject.CreateInstance<Item>();

                newItem.name = data.itemNames[i];
                newItem.ItemCount = data.itemCounts[i];
                string itemAssetPath = "Items/" + newItem.name;
                Item newItemClass = (Item)Resources.Load(itemAssetPath);
                if (newItemClass == null)
                {
                    continue;
                }
                // load the asset to set other properties easily
                newItem.ItemName = newItemClass.ItemName;
                newItem.ItemCost = newItemClass.ItemCost; // set
                newItem.ItemDetailType = newItemClass.ItemDetailType;
                newItem.ItemImage = newItemClass.ItemImage;
                newItem.ItemPrefab = newItemClass.ItemPrefab;
                newItem.IsAlreadyGet = true;
                newItem.ItemInfo = newItemClass.ItemInfo;
                newItem.Type = newItemClass.Type;
                newItem.name = data.itemNames[i];
                itemSlots[i].AddItemOnLoad(newItem, newItem.ItemCount);
            }
        }
    }

    private void LoadQuickItems(PlayerData data) // data의 아이템이 아이템인지 장비인지 구별해야함. isEquipment 함수를 자료형에 추가하고. 또 장비의 슬롯 인덱스도 저장해야함.
    {
        for (int i = 0; i < data.quickItemNames.Length; i++)
        {
            Slot refer_slot = uiManagerObject.GetComponentsInChildren<Slot>(true)[data.itemSlotReferencesIdx[i]];
            refer_slot.ItemToolTip = uiManagerObject.GetComponentInChildren<ItemToolTip>(true);
            var quickItemSlots = uiManagerObject.GetComponentsInChildren<ItemQuickSlot>(true);
            if (data.quickItemNames[i] != null)
            {
                // Item 객체 생성
                Item newItem = ScriptableObject.CreateInstance<Item>();
                newItem.name = data.quickItemNames[i];
                newItem.ItemCount = data.quickItemCounts[i];

                string itemAssetPath = "Items/" + newItem.name;
                Item newItemClass = (Item)Resources.Load(itemAssetPath);
                if (newItemClass == null)
                {
                    continue;
                }
                if (newItemClass != null)
                {
                    newItem.ItemName = newItemClass.ItemName;
                    newItem.ItemCost = newItemClass.ItemCost;
                    newItem.ItemDetailType = newItemClass.ItemDetailType;
                    newItem.ItemImage = newItemClass.ItemImage;
                    newItem.ItemPrefab = newItemClass.ItemPrefab;
                    newItem.IsAlreadyGet = true;
                    newItem.ItemInfo = newItemClass.ItemInfo;
                    newItem.Type = newItemClass.Type;

                    quickItemSlots[i].AddItemOnLoad(newItem, data.quickItemCounts[i], refer_slot);
                }
            }

        }
    }

    void ActivatePauseMenu()
    {
        if (PauseMenuInstance != null)
        {
            return;
        }
        if (PaueseMenu != null)
        {
            PauseMenuInstance = Instantiate(PaueseMenu, new Vector3(0, 0, 0), Quaternion.identity);
            PauseMenuInstance.SetActive(true);
            Button[] PauseMenuBtns = PauseMenuInstance.GetComponentsInChildren<Button>();
            PauseMenuBtns[0].onClick.AddListener(InactivatePauseMenu);
            PauseMenuBtns[1].onClick.AddListener(DoSave);
            PauseMenuBtns[2].onClick.AddListener(GoBackHome);
        }
    }

    public void GoBackHome()
    {
        GameObject soundManager = GameObject.FindWithTag("SoundManager");
        GameObject uiManagerObject = GameObject.FindWithTag("UIManager");
        GameObject questManager = GameObject.FindWithTag("QuestManager");
        GameObject saveManager = GameObject.FindWithTag("SaveManager");
        GameObject player = GameObject.FindWithTag("Player");
        Destroy(soundManager);
        Destroy(uiManagerObject);
        Destroy(questManager);
        Destroy(saveManager);
        Destroy(player);
        SceneManager.LoadScene("Main Menu Scene");
    }
    void InactivatePauseMenu()
    {
        if (PauseMenuInstance != null)
        {
            Destroy(PauseMenuInstance);
            PauseMenuInstance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 현재 씬의 이름이 특정 씬의 이름과 일치하는지 확인합니다.
        if (scene.name == "Main Menu Scene")
        {
            // 원하는 오브젝트를 파괴합니다.
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트에서 메서드를 제거합니다.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
