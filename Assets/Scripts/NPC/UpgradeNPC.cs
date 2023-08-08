using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using System.Threading.Tasks;
public class UpgradeNPC : MonoBehaviour
{
    //@notion ???????? ????????
    public PPC721Contract PPC721Contract;
    public PPCTokenContract PPCTokenContract;

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
    private bool isTaskEnd = false;
    // Start is called before the first frame update
    void Start()
    {
        //@notion ???????? ????
        PPCTokenContract.Initialize();
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
        //@notion ???????? ??????
        PPCTokenContract = GetComponent<PPCTokenContract>();
        PPC721Contract = GetComponent<PPC721Contract>();

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
        StartCoroutine(PayTokenAndUpgrade());
    }

    public IEnumerator PayTokenAndUpgrade()
    {
        GameObject canvas = GameObject.Find("Canvas");
       HTTPClient.instance.spinner = Instantiate(HTTPClient.instance.progressSpinner, canvas.transform);

        HTTPClient.instance.spinner.SetActive(true);
        //@notion Testcode
        //yield return new WaitForSeconds(1f);
        //isTaskEnd = true;
        
        //@notion paytoken
        yield return StartCoroutine(payToken("0xE503081665f268c99ff22F45Df5FC8f3A21Ef0C8", "0x30018fC76ca452C1522DD9C771017022df8b2321", 5));

        if (isTaskEnd)
        {
            print("tt");
            //Upgrade
            float upgradeProb = upgradeData.GetProbForUpgrade(selectWeaponUpgrade) + selectIncreaseProb;
            if (upgradeProb > 100)
                upgradeProb = 100;
            upgradeProb /= 100f;

            if (Random.value < upgradeProb)
            {
                UpgradeSuccess(selectWeapon);
            }
            else
            {
                UpgradeFail(selectWeapon);
            }
            ItemManager.instance.UsePPC(upgradeData.GetCostForUpgrade(selectWeaponUpgrade));


            ChangeNature(selectWeapon);
            RefreshPanel();

            

            string body = JsonUtility.ToJson(selectWeapon);

            HTTPClient.instance.PUT("https://breadmore.azurewebsites.net/api/Weapon_Data/" + selectWeapon.weapon_id,
                body,
                delegate(string www)
                {
                    HTTPClient.instance.spinner.SetActive(false);
                });
        }
    }

    public void ChangeNature(WeaponData weaponData)
    {
        if (weaponData.weapon_upgrade >= 10)
        {
            int rand = Random.Range(0, 5);
            weaponData.weapon_element = (WeaponNature)rand;
        }
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

    public IEnumerator payToken(string sender, string recipient, decimal amount)
    {
        print("task on");
        isTaskEnd = false;

        yield return StartCoroutine(PPCTokenContract.Approve("0x52923645D1a2706c3B40C3F1Dc6AA170f7BEf10D", amount, (Address, ex) =>
        {
            Debug.Log($"Approve Contract Address: {Address}");
        }));

        yield return StartCoroutine(PPC721Contract.PPCTransferFrom(sender, recipient, amount, (Address, ex) =>
        {
            Debug.Log($"PPCTransferFrom Contract Address: {Address}");
        }));
        print("task off");
        isTaskEnd = true;
    }

    public IEnumerator UpdateDnft(BigInteger tokenId, string tokenURI)
    {
        yield return StartCoroutine(PPC721Contract.UpdataNFT(tokenId, tokenURI, (Address, ex) =>
        {
            Debug.Log($"UpdataNFT Contract Address: {Address}");
        }));
    }

}
