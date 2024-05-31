using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public EquipmentSlot HelmetSlot;
    public EquipmentSlot BodySlot;
    public EquipmentSlot PantsSlot;
    public EquipmentSlot WeaponSlot;
    public EquipmentSlot GloveSlot;
    public EquipmentSlot ShoesSlot;
    public EquipmentSlot PetSlot;
    public EquipmentSlot TrinketSlot;
    private PlayerStatus playerStatus;

    void Start() {
        playerStatus = FindObjectOfType<PlayerStatus>();
        if (!playerStatus.IsLoaded){
        HelmetSlot.InitialEquip();

        BodySlot.InitialEquip();

        PantsSlot.InitialEquip();

        WeaponSlot.InitialEquip();

        GloveSlot.InitialEquip();

        ShoesSlot.InitialEquip();

        PetSlot.InitialEquip();

        TrinketSlot.InitialEquip();
        }
        HelmetSlot.ConnectUIManager();
        BodySlot.ConnectUIManager();
        PantsSlot.ConnectUIManager();
        WeaponSlot.ConnectUIManager();
        GloveSlot.ConnectUIManager();
        ShoesSlot.ConnectUIManager();
        PetSlot.ConnectUIManager();
        TrinketSlot.ConnectUIManager();
    }
}
