using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MarketNPC : MonoBehaviour
{
    private static MarketNPC _instance;
    public static MarketNPC instance
    {

        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<MarketNPC>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }

    private bool IsBuy = false;
    private bool IsSell = false;
    private bool IsMy = false;

    public Image selectWeaponImage;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI natureText;
    public TextMeshProUGUI durText;

    public TMP_InputField inputPrice;

    private WeaponData selectWeapon;
    private EquipSlot equipSlot;

    public MarketSlot selectMarket;

    public void UnSelect()
    {
        IsBuy = false;
        IsSell = false;
        IsMy = false;
        UIManager.instance.Buy.SetActive(false);
        UIManager.instance.Sell.SetActive(false);
        UIManager.instance.My.SetActive(false);
    }
    public void SelectBuy()
    {
        UnSelect();
        IsBuy = true;
        UIManager.instance.Buy.SetActive(true);
    }

    public void SelectSell()
    {
        UnSelect();
        IsSell = true;
        UIManager.instance.Sell.SetActive(true);
    }

    public void SelectMy()
    {
        UnSelect();
        IsMy = true;
        UIManager.instance.My.SetActive(true);

    }

    
    public void SelectWeapon(Button button)
    {
        equipSlot = button.GetComponent<EquipSlot>();
        selectWeapon = equipSlot.weaponData;
        selectWeaponImage.sprite = equipSlot.itemImage.sprite;
        levelText.text = selectWeapon.weapon_upgrade.ToString();
        healthText.text = selectWeapon.weapon_hp.ToString();
        damageText.text = selectWeapon.weapon_atk.ToString();
        natureText.text = selectWeapon.weapon_element.ToString();
        durText.text = selectWeapon.weapon_durability.ToString();

        print(selectWeapon.weapon_id);
    }

    public void CloseBuyPanel()
    {
        UIManager.instance.BuyPanel.SetActive(false);
    }

    public void BuyThisItem()
    {
        
        if (selectMarket.marketData.weapon_cost < ItemManager.instance.PPC)
        {

            //weapon_tb owner -> me
            WeaponTB weaponTB = new WeaponTB();
            weaponTB.weapon_id = selectMarket.marketData.weapon_id;
            weaponTB.weapon_owner = LoginManager.instance.PlayerID;

            string url = "https://breadmore.azurewebsites.net/api/Weapon_TB/" + weaponTB.weapon_id;
            string jsonData = JsonUtility.ToJson(weaponTB);
            HTTPClient.instance.PUT(url, jsonData, (response) =>
            {
                Debug.Log("PUT Response: " + response);
                // Process the response here
                string url2 = "https://breadmore.azurewebsites.net/api/Weapon_Market/" + weaponTB.weapon_id;
                HTTPClient.instance.DELETE(url2, (response) =>
                {
                    Debug.Log("PUT Response: " + response);
                    // Process the response here
                });

                WeaponManager.instance.GetWeaponList();
            });


            WeaponSale weaponSale = new WeaponSale();
            weaponSale.weapon_Sale = 0;
            string jsonData3 = JsonUtility.ToJson(weaponSale);
            string url3 = "https://breadmore.azurewebsites.net/api/Weapon_Data/weapon/" + weaponTB.weapon_id + "?weapon_sale=0"; // Change this to your specific API endpoint

            HTTPClient.instance.PUT(url3, jsonData3, (response) =>
            {
                Debug.Log("PUT Response: " + response);


                ActionController.instance.Refresh();
                // Process the response here
            });
            //delete market item

            //
            ItemManager.instance.UsePPC(selectMarket.marketData.weapon_cost);


        }





        CloseBuyPanel();
    }

    public void SellWeaponToMarket()
    {
        MarketData marketData = new MarketData();
        WeaponSale weaponSale = new WeaponSale();
        marketData.weapon_id = selectWeapon.weapon_id;

        marketData.weapon_cost = 0;
        if(int.TryParse(inputPrice.text, out int price))
        {
            marketData.weapon_cost = price;
        }
        marketData.weapon_owner = LoginManager.instance.PlayerID;
        string jsonData2 = JsonUtility.ToJson(marketData);
        string url2 = "https://breadmore.azurewebsites.net/api/Weapon_Market"; // Change this to your specific API endpoint



        HTTPClient.instance.POST(url2, jsonData2, (response) =>
          {
              Debug.Log("POST Response: " + response);
              // Sending a PUT request

              string jsonData = JsonUtility.ToJson(weaponSale);
              string url = "https://breadmore.azurewebsites.net/api/Weapon_Data/weapon/" + selectWeapon.weapon_id + "?weapon_sale=1"; // Change this to your specific API endpoint

              HTTPClient.instance.PUT(url, jsonData, (response) =>
              {
                  Debug.Log("PUT Response: " + response);
                  weaponSale.weapon_Sale = 1;
                  equipSlot.ClearSlot();
                  WeaponManager.instance.ChangeWeaponData(selectWeapon);

                  ActionController.instance.Refresh();
                  // Process the response here
              });

          });
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
