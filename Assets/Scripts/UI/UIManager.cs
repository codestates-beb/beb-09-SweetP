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
    private bool IsRanking = false;

    private AudioSource audioSource;
    public AudioClip inventoryClip;
    private ObjectEventSystem eventSystem;
    [Header("Inventory")]
    public GameObject Inventory;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI ppcText;
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
    public GameObject MarketPanel;
    public GameObject UpgradePanel;

    public bool IsContract = false;
    public bool IsOpenPanel = false;

    [Header("Battle UI")]
    public GameObject GroundPanel;
    public GameObject RaidPanel;

    [Header("Shop UI")]
    public TextMeshProUGUI ItemCostText;
    public TextMeshProUGUI ItemCountText;
    public TextMeshProUGUI MyGoldText;

    [Header("Upgrade UI")]
    public Image UpgradeWeapon;
    public Image UpgradeScroll;
    public TextMeshProUGUI PPCText;
    public TextMeshProUGUI CurrentUpgrade;
    public TextMeshProUGUI NextUpgrade;
    public GameObject DecideOn;
    public GameObject DecideOff;

    [Header("Market UI")]
    public GameObject Buy;
    public GameObject Sell;
    public GameObject My;
    public Image Select;
    public Image UnSelect;
    public GameObject BuyPanel;

    [Header("Ranking")]
    public GameObject RankingPanel;
    
    [Header("Monster")]
    public Material material;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        eventSystem = Inventory.GetComponent<ObjectEventSystem>();
        if (eventSystem != null)
        {
            eventSystem.ObjectSetActiveChanged += OnInventorySetActiveChanged;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //UI
        RefreshPotionCount();
        goldText.text = ItemManager.instance.itemData.player_gold.ToString();
        ppcText.text = ItemManager.instance.PPC.ToString();

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

    private async void OnInventorySetActiveChanged(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(inventoryClip);
        goldText.text = ItemManager.instance.itemData.player_gold.ToString();
        ppcText.text = ItemManager.instance.PPC.ToString();


        await ActionController.instance.RefreshAll();
    }

 

    public void RefreshPotionCount()
    {
        potionText.text = ItemManager.instance.itemData.player_potion.ToString();
        string url = "";
        string body = "";
        //HTTPClient.instance.PUT(url, body, delegate (string www) { });
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            IsInventory = !IsInventory;
            Inventory.SetActive(IsInventory);

            if (!IsRanking)
                GameManager.instance.IsUI = IsInventory;
            
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            IsRanking = !IsRanking;
            RankingPanel.SetActive(IsRanking);

            if (!IsInventory)
                GameManager.instance.IsUI = IsInventory;
        }
    }
}
