using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EquipSlot : MonoBehaviour
    ,IPointerEnterHandler
    ,IPointerExitHandler
{

    public WeaponData weaponData;
    public Image itemImage;
    private EquipSlot equipSlot;

    private void SetColor(float alpha)
    {
        Color color = itemImage.color;
        color.a = alpha;
        itemImage.color = color;
    }

    public void AddWeapon(WeaponData _weaponData)
    {
        if (_weaponData.weapon_sale == 0)
        {
            print("Slot : Add weapon" + _weaponData.weapon_id);
            weaponData = _weaponData;
            switch (weaponData.weapon_type)
            {
                //sword
                case 0:
                    itemImage.sprite = Item.instance.Sword;
                    break;
                //bow
                case 1:
                    itemImage.sprite = Item.instance.Bow;
                    break;
                //magic
                case 2:
                    itemImage.sprite = Item.instance.Magic;
                    break;
            }

            SetColor(1);
        }
    }

    public void OpenPanel()
    {
        if (weaponData.weapon_id != 0)
            UIManager.instance.OpenWeaponSelectPanel(equipSlot);
    }

    public void ClosePanel()
    {
            UIManager.instance.CloseWeaponSelectPanel();
    }
    public void EquipWeapon()
    {
        if (weaponData.weapon_id != 0)
        {
            WeaponManager.instance.EquipWeapon(weaponData);
            switch (WeaponManager.instance.curruentWeaponData.weapon_type)
            {
                case 0:
                    GameObject weapon = Instantiate(WeaponManager.instance.Sword, PlayerAttack.instance.pivotWeaponR);

                    PlayerAttack.instance.objWeapon = PlayerAttack.instance.pivotWeaponR.GetChild(0).gameObject;
                    PlayerAttack.instance.colliderWeapon = PlayerAttack.instance.objWeapon.GetComponent<BoxCollider>();

                    PlayerAttack.instance.colliderWeapon.enabled = false;
                    PlayerAttack.instance.IsWeaponEquip = true;
                    Player.instance.ChangeHealthWithWeapon();
                    break;

                //bow
                case 1:
                    break;

                //magic
                case 2:
                    break;
            }
        }
    }
    public void ClearSlot()
    {
        weaponData = new WeaponData();
        itemImage.sprite = null;
        SetColor(0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(weaponData.weapon_id !=0)
        UIManager.instance.OpenWeaponInfoPanel(equipSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (weaponData.weapon_id != 0)
            UIManager.instance.CloseWeaponInfoPanel();
    }

    // Start is called before the first frame update
    void Start()
    {
        equipSlot = gameObject.GetComponent<EquipSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
