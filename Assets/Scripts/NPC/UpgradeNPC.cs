using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNPC : MonoBehaviour
{
    public GameObject WeaponSelectPanel;
    public GameObject ScrollSelectPanel;

    private bool IsWeaponSelect = false;
    private bool IsScrollSelect = false;

    private WeaponData selectWeapon;
    private int selectWeaponUpgrade;

    private ScrollData selectScroll;
    private int selectIncreaseProb;
    private ObjectEventSystem eventSystem;
    private UpgradeData upgradeData;

    // Start is called before the first frame update
    void Start()
    {
        upgradeData = GetComponent<UpgradeData>();
    }

    // Update is called once per frame
    void Update()
    {
        CanUpgradeCheck();
    }

    private void OnUpgradeSetActiveChanged(object sender, System.EventArgs e)
    {
        UIManager.instance.DecideOff.SetActive(true);
        UIManager.instance.DecideOn.SetActive(false);

        UIManager.instance.UpgradeWeapon.sprite = Item.instance.NullIcon;
        UIManager.instance.UpgradeScroll.sprite = Item.instance.NullIcon;
        UIManager.instance.PPCText.text = "0";
        UIManager.instance.CurrentUpgrade.text = "0";
        UIManager.instance.NextUpgrade.text = "0";
        selectWeapon = null;
        selectWeaponUpgrade = 0;
        selectScroll = null;
        selectIncreaseProb = 0;

        IsWeaponSelect = false;
        IsScrollSelect = false;

        print("hello");
    }

    private void RefreshPanel()
    {
        UIManager.instance.PPCText.text = upgradeData.GetCostForUpgrade(selectWeaponUpgrade).ToString();
        UIManager.instance.CurrentUpgrade.text = selectWeaponUpgrade.ToString();
        UIManager.instance.NextUpgrade.text = (selectWeaponUpgrade + 1).ToString();
    }
    private void Awake()
    {

        eventSystem = UIManager.instance.UpgradePanel.GetComponent<ObjectEventSystem>();

        if (eventSystem != null)
        {
            eventSystem.ObjectSetActiveChanged += OnUpgradeSetActiveChanged;
        }

    }

    public void OpenWeaponSelect()
    {
        WeaponSelectPanel.SetActive(true);

    }
    public void CloseWeaponSelect(Button button)
    {
        EquipSlot equipSlot = button.GetComponent<EquipSlot>();
        WeaponSelectPanel.SetActive(false);

        if (equipSlot.weaponData.weapon_id != 0)
        {
            IsWeaponSelect = true;
            UIManager.instance.UpgradeWeapon.sprite = equipSlot.itemImage.sprite;
            selectWeapon = equipSlot.weaponData;
            selectWeaponUpgrade = selectWeapon.weapon_upgrade;
            RefreshPanel();
        }

    }

    public void OpenItemSelect()
    {
        ScrollSelectPanel.SetActive(true);
    }
    public void CloseItemSelect(Button button)
    {
        ItemSlot itemSlot = button.GetComponent<ItemSlot>();
        ScrollSelectPanel.SetActive(false);

        if (itemSlot.scrollData.count != 0)
        {
            IsScrollSelect = true;
            UIManager.instance.UpgradeScroll.sprite = itemSlot.itemImage.sprite;
            selectScroll = itemSlot.scrollData;
            selectIncreaseProb = selectScroll.IncreseProb;
        }

        
    }

    private void CanUpgradeCheck()
    {
        if (IsWeaponSelect && IsScrollSelect)
        {

            if (ItemManager.instance.PPC > upgradeData.GetCostForUpgrade(selectWeaponUpgrade))
            {
                UIManager.instance.DecideOn.SetActive(true);
                UIManager.instance.DecideOff.SetActive(false);
            }
        }
    }


    public void UpgradeWeapon()
    {

        //Upgrade
        float upgradeProb = upgradeData.GetProbForUpgrade(selectWeaponUpgrade) + selectIncreaseProb;
        if (upgradeProb > 100)
            upgradeProb = 100;
        upgradeProb /= 100f;

        if(Random.value < upgradeProb)
        {
            UpgradeSuccess(selectWeapon);
        }
        else
        {
            UpgradeFail(selectWeapon);
        }
        ItemManager.instance.UsePPC(upgradeData.GetCostForUpgrade(selectWeaponUpgrade));
        RefreshPanel();
    }

    private void UpgradeFail(WeaponData weaponData)
    {
        if(weaponData.weapon_upgrade>0)
        weaponData.weapon_upgrade--;

        WeaponManager.instance.ChangeWeaponData(weaponData);
        selectWeapon = weaponData;
        selectWeaponUpgrade = selectWeapon.weapon_upgrade;
    }

    private void UpgradeSuccess(WeaponData weaponData)
    {
        weaponData.weapon_upgrade++;

        WeaponManager.instance.ChangeWeaponData(weaponData);
        selectWeapon = weaponData;
        selectWeaponUpgrade = selectWeapon.weapon_upgrade;
    }
}
