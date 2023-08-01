using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<UIManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }

    private bool IsInventory = false;

    [Header("Inventory")]
    public GameObject Inventory;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI jewelText;
    public TextMeshProUGUI potionText;

    [Header("Weapon Select Panel")]
    public GameObject WeaponSelectPanel;

    [Header("Weapon Info Panel")]
    public GameObject WeaponInfoPanel;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponLevelText;
    public TextMeshProUGUI weaponATKText;
    public TextMeshProUGUI weaponHPText;
    public TextMeshProUGUI weaponNatureText;
    public TextMeshProUGUI weaponDurText;
    public Image WeaponImage;
    private EquipSlot equipSlot;

    [Header("NPC UI")]
    public GameObject ContractPanel;
    public TextMeshProUGUI ContractNPCNameText;
    public GameObject ShopPanel;
    public GameObject BattlePanel;
    public GameObject ChangePanel;

    public bool IsContract = false;
    public bool IsOpenPanel = false;

    [Header("Battle UI")]
    public GameObject GroundPanel;
    public GameObject RaidPanel;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        potionText.text = ItemManager.instance.itemData.player_potion.ToString();
        goldText.text = ItemManager.instance.itemData.player_gold.ToString();
    }

    public void OpenWeaponInfoPanel(EquipSlot _equipSlot)
    {
        equipSlot = _equipSlot;
        weaponNameText.text = equipSlot.weaponData.weapon_id.ToString();
        weaponLevelText.text = equipSlot.weaponData.weapon_upgrade.ToString();
        weaponATKText.text = equipSlot.weaponData.weapon_atk.ToString();
        weaponHPText.text = equipSlot.weaponData.weapon_hp.ToString();
        weaponNatureText.text = equipSlot.weaponData.weapon_element.ToString();
        weaponDurText.text = equipSlot.weaponData.weapon_durability.ToString();
        WeaponImage.sprite = equipSlot.itemImage.sprite;
        WeaponInfoPanel.SetActive(true);
    }

    public void CloseWeaponInfoPanel()
    {
        WeaponInfoPanel.SetActive(false);
    }

    public void OpenWeaponSelectPanel(EquipSlot _equipSlot)
    {
        equipSlot = _equipSlot;
        Vector3 pos = equipSlot.transform.position;
        WeaponSelectPanel.transform.position = pos;
        WeaponSelectPanel.SetActive(true);
    }

    public void CloseWeaponSelectPanel()
    {
        WeaponSelectPanel.SetActive(false);
    }

    public void EquipOk()
    {
        equipSlot.EquipWeapon();
        WeaponSelectPanel.SetActive(false);
    }

    public void SelectBattleType()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (IsInventory)
            {
                Inventory.SetActive(false);
                IsInventory = false;


                GameManager.instance.IsUI = false;
            }
            else
            {
                Inventory.SetActive(true);
                IsInventory = true;

                GameManager.instance.IsUI = true;
            }
        }
    }
}
