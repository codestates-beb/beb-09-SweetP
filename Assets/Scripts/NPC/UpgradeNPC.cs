using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNPC : MonoBehaviour
{
    public GameObject WeaponSelectPanel;
    public GameObject ScrollSelectPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenWeaponSelect()
    {
        WeaponSelectPanel.SetActive(true);
    }
    public void CloseWeaponSelect(Button button)
    {
        EquipSlot equipSlot = button.GetComponent<EquipSlot>();
        UIManager.instance.UpgradeWeapon.sprite = equipSlot.itemImage.sprite;
        WeaponSelectPanel.SetActive(false);

    }

    public void OpenItemSelect()
    {
        ScrollSelectPanel.SetActive(true);
    }
    public void CloseItemSelect(Button button)
    {
        ItemSlot itemSlot = button.GetComponent<ItemSlot>();
        UIManager.instance.UpgradeScroll.sprite = itemSlot.itemImage.sprite;
        ScrollSelectPanel.SetActive(false);

    }
}
