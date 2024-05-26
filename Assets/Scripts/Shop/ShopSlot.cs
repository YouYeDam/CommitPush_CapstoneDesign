using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item Item; // 판매하는 아이템
    public Image ItemImage;  // 아이템의 이미지
    public BuyItemInputField BuyItemInputField;
    CantBuyAlarm CantBuyAlarm;
    ItemToolTip ShopItemToolTip;
    PlayerMovement PlayerMovement;
    PlayerGetItem PlayerGetItem;
    PlayerMoney PlayerMoney;

    // 더블클릭 기능 구현 변수
    private int ClickCount = 0;
    private float LastClickTime = 0f;
    private const float DoubleClickTime = 0.2f; // 더블 클릭 간격


    void Start() {
        BuyItemInputField = FindObjectOfType<BuyItemInputField>();
        CantBuyAlarm = FindAnyObjectByType<CantBuyAlarm>();

        GameObject ShopToolTipObject = GameObject.Find("ShopItemToolTip");
        if (ShopToolTipObject != null) {
        ShopItemToolTip = ShopToolTipObject.GetComponent<ItemToolTip>();
        }
        PlayerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        PlayerGetItem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGetItem>();
        PlayerMoney = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoney>();
    }

    public void SetColor(float Alpha){ // 아이템 이미지의 투명도 조절
        Color Color = ItemImage.color;
        Color.a = Alpha;
        ItemImage.color = Color;
    }

    public void AddItem(Item Item) { // 상점에 새로운 아이템 슬롯 추가
        this.Item = Item;
        ItemImage.sprite = this.Item.ItemImage;
        SetColor(1);
    }

    public void ClearSlot() { // 해당 슬롯 삭제
        Item = null;
        ItemImage.sprite = null;
        SetColor(0);
    }
    public void OnPointerEnter(PointerEventData eventData) {
        if (Item != null) {
            ShopItemToolTip.ShowToolTip(Item);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        ShopItemToolTip.HideToolTip();
    }

    public void OnPointerClick(PointerEventData eventData) {
        ClickCount++;
        if (ClickCount == 1)
        {
            LastClickTime = Time.time;
        }
        else if (ClickCount == 2 && Time.time - LastClickTime < DoubleClickTime)
        {
            OnDoubleClick(); // 더블 클릭 발생
            ClickCount = 0; // 클릭 카운트 초기화
        }
        else if (Time.time - LastClickTime > DoubleClickTime)
        {
            ClickCount = 1; // 시간 초과로 다시 1부터 카운트
            LastClickTime = Time.time;
        }
    }

    void OnDoubleClick() // 슬롯 더블클릭
    {
        if (Item == null || !PlayerMovement.IsAlive) {
            return;
        }

        if (Item.Type == Item.ItemType.Used) // 소비 아이템시 실행
        {
            BuyItemInputField.OpenInputField(this);
        }
        else if (Item.Type == Item.ItemType.Equipment) // 장비 아이템시 실행
        {
            if (PlayerMoney.Bit >= Item.ItemCost) {
                PlayerMoney.Bit -= Item.ItemCost;
                PlayerGetItem.InventoryScript.AcquireItem(Item);
            }
            else {
                CantBuyAlarm.OpenCantBuyAlarm();
            }
        }
    }

    public void BuyManyItem(int BuyItemCount) {
        int TotalItemCost = Item.ItemCost * BuyItemCount;
        if (PlayerMoney.Bit >= TotalItemCost) {
            PlayerMoney.Bit -= TotalItemCost;
            PlayerGetItem.InventoryScript.AcquireItem(Item, BuyItemCount);
        }
        else {
                CantBuyAlarm.OpenCantBuyAlarm();
        }
    }

}