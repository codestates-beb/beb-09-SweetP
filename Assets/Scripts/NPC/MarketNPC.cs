using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Numerics;
using System.Threading.Tasks;

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

    //@notion 컨트랙트 컴포넌트
    public PPC721Contract PPC721Contract;
    public PPCTokenContract PPCTokenContract;

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

    private void Awake()
    {
        //@notion 컨트랙트 초기화
        PPCTokenContract = GetComponent<PPCTokenContract>();
        PPC721Contract = GetComponent<PPC721Contract>();

    }

    void Start()
    {
        //@notion 컨트랙트 연결
        PPCTokenContract.Initialize();
    }

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

    public async void BuyThisItem()
    {
        
        if (selectMarket.marketData.weapon_cost < ItemManager.instance.PPC)
        {

            //weapon_tb owner -> me
            WeaponTB weaponTB = new WeaponTB();
            weaponTB.weapon_id = selectMarket.marketData.weapon_id;
            weaponTB.weapon_owner = LoginManager.instance.PlayerID;

            string url = "https://breadmore.azurewebsites.net/api/Weapon_TB/" + weaponTB.weapon_id;
            string jsonData = JsonUtility.ToJson(weaponTB);

            StartCoroutine(BuyWeapon(selectWeapon.weapon_id));

            HTTPClient.instance.PUT(url, jsonData, (response) =>
            {
                // Process the response here
                string url2 = "https://breadmore.azurewebsites.net/api/Weapon_Market/" + weaponTB.weapon_id;
                HTTPClient.instance.DELETE(url2, (response) =>
                {
                    // Process the response here
                });
            });


            WeaponSale weaponSale = new WeaponSale();
            weaponSale.weapon_Sale = 0;
            string jsonData3 = JsonUtility.ToJson(weaponSale);
            string url3 = "https://breadmore.azurewebsites.net/api/Weapon_Data/weapon/" + weaponTB.weapon_id + "?weapon_sale=0"; // Change this to your specific API endpoint

            HTTPClient.instance.PUT(url3, jsonData3, (response) =>
            {
                // Process the response here
                WeaponManager.instance.Refresh();
                WeaponManager.instance.GetWeaponList();
            });

            ItemManager.instance.UsePPC(selectMarket.marketData.weapon_cost);

            
            await ActionController.instance.RefreshAll();

        }

        CloseBuyPanel();

    }

    public async void SellWeaponToMarket()
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

        StartCoroutine(SaleWeapon(selectWeapon.weapon_id,marketData.weapon_cost));

        HTTPClient.instance.POST(url2, jsonData2, (response) =>
          {
              // Sending a PUT request

              string jsonData = JsonUtility.ToJson(weaponSale);
              string url = "https://breadmore.azurewebsites.net/api/Weapon_Data/weapon/" + selectWeapon.weapon_id + "?weapon_sale=1"; // Change this to your specific API endpoint

              HTTPClient.instance.PUT(url, jsonData, (response) =>
              {
                  weaponSale.weapon_Sale = 1;
                  equipSlot.ClearSlot();

                  WeaponManager.instance.ChangeWeaponData(selectWeapon);
                  WeaponManager.instance.Refresh();
                  WeaponManager.instance.GetWeaponList();
                  // Process the response here
                  print($"sellweapon data : {response}");
              });


          });
        await ActionController.instance.RefreshAll();
        UIManager.instance.MarketPanel.SetActive(false);

        

    }

    //@notion 무기거래소 UI켜면 실행되는 함수
    //1.판매로 올라온 무기 리스트 띄우기
    public IEnumerator GetSaleWeapons()
    {
        yield return StartCoroutine(PPC721Contract.GetSaleNftTokenIds((Weapons, ex) =>
        {
            if (ex == null)
            {
                int[] WeaponsList = Weapons;
                for (int i = 0; i < WeaponsList.Length; i++)
                {
                    StartCoroutine(GetWeaponTokenURI(WeaponsList[i]));
                }
            }
            else
            {
                Debug.Log(ex);
            }
        }));
    }

    private IEnumerator GetWeaponTokenURI(int tokenId)
    {
        yield return StartCoroutine(PPC721Contract.GetTokenURI(tokenId, (url, ex) =>
        {
            string WeaponURl = url;
            Debug.Log($"Token {tokenId} URL: {WeaponURl}");
        }));
    }

    //@notion 무기거래
    //무기 거래 상세 페이지 들어가서 buy버튼을 누르면 시행되는 함수

    public IEnumerator BuyWeapon(BigInteger tokenId)
    {
        //@notion 무기를토큰으로 사기위한 721컨트랙트에 토큰꺼낼수 있는양 정함
        yield return StartCoroutine(PPCTokenContract.Approve("0xb666d55294EfA8A8CCaCFdf1485e5D7484B92684", 20, (Address, ex) =>
        {
            Debug.Log($"Contract Address: {Address}");
        }));
        yield return StartCoroutine(PPC721Contract.BuyNftToken(tokenId, (Address, ex) =>
        {
            Debug.Log($"Contract Address: {Address}");
        }));

    }


    //@notion 무기 판매 등록
    //무기 판매 버튼을 누르면 시행되는 함수
    public IEnumerator SaleWeapon(BigInteger tokenId, BigInteger price)
    {
        yield return StartCoroutine(PPC721Contract.SaleNftToken(tokenId, price, (Address, ex) =>
        {
            Debug.Log($"Contract Address: {Address}");
            Debug.Log($"SaleWeapon {tokenId} / price : {price}");
        }));
    }

    public IEnumerator GetIsSaleWeapon(BigInteger tokenId)
    {
        yield return StartCoroutine(PPC721Contract.GetIsSale(tokenId, (isSale, ex) =>
        {
            if (ex == null)
            {
                bool isTokenSale = isSale;
                Debug.Log($"{tokenId} Token Sale : {isTokenSale}");
            }
            else
            {
                Debug.Log(ex);
            }
        }));
    }


    //@notion weapon metadata update

    public IEnumerator UpdateDnft(BigInteger tokenId, string tokenURI)
    {
        yield return StartCoroutine(PPC721Contract.UpdataNFT(tokenId, tokenURI, (Address, ex) =>
        {
            Debug.Log($"UpdataNFT Contract Address: {Address}");
        }));
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
