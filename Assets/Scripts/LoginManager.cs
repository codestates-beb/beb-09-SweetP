using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using NFTStorage.JSONSerialization;

public class LoginManager : MonoBehaviour
{
    public static LoginManager _instance;
    public static LoginManager instance
    {

        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<LoginManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    //@notion 컨트랙트 컴포넌트
    public PPC721Contract PPC721Contract;

    //@IPFS
    string fullPath;
    public NFTStorage.NFTStorageClient NSC;

    public string body;
    private MetaDataWeapon metaDataWeapon;

    public TextMeshProUGUI PlayerNameText;

    //[HideInInspector]
    [HideInInspector]
    public string PlayerAddress;
    [HideInInspector]
    public int PlayerID;
    [HideInInspector]
    public string PlayerName;

    [Header("New Account")]
    public GameObject newstartPanel;
    public GameObject newAccountButton;
    public TMP_InputField inputAddress;
    public TMP_InputField inputName;


    [Header("Login")]
    public GameObject loginPanel;
    public GameObject loginButton;
    public TMP_InputField inputLoginAddress;
    private void Awake()
    {
        PPC721Contract = GetComponent<PPC721Contract>();
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        metaDataWeapon = new MetaDataWeapon();
        //test user
    }

    private void HandleData(string jsonData)
    {
        PlayerTB playerTB = JsonUtility.FromJson<PlayerTB>(jsonData);

        PlayerID = playerTB.player_id;
        PlayerName = playerTB.player_name;
    }

    public void Login()
    {
        if (inputLoginAddress.text.Length == 0)
        {
            return;
        }
        PlayerAddress = inputLoginAddress.text;
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/player_tb/address/" + PlayerAddress, delegate (string www)
        {
            HandleData(www);

            PlayerNameText.text = "Player Name : " + PlayerName;
            loginButton.SetActive(false);
            loginPanel.SetActive(false);
            newAccountButton.SetActive(false);
        });
        StartCoroutine(setToken());


    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
    }

    public void OpenNewstartPanel()
    {
        newstartPanel.SetActive(true);
    }

    public PlayerTB HandleNewAccountInfo(string www)
    {
        return JsonUtility.FromJson<PlayerTB>(www);
    }

    public WeaponData HandleNewAccountWeapon(string www)
    {
        string jsonData = RemoveSquareBrackets(www);
        print(jsonData);
        WeaponTB weaponTB = JsonUtility.FromJson<WeaponTB>(jsonData);
        WeaponData weaponData = new WeaponData();

        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Weapon_Data/" + weaponTB.weapon_id, delegate (string www) {

            weaponData = JsonUtility.FromJson<WeaponData>(www);

        });

        return weaponData;
    }

    private string RemoveSquareBrackets(string jsonString)
    {
        // 문자열에서 대괄호 제거
        string result = jsonString.Trim('[', ']');

        return result;
    }

    public void NewStart()
    {

        PlayerTB playerTB = new PlayerTB();
        playerTB.player_address = inputAddress.text;
        playerTB.player_name = inputName.text;

        PlayerTB newPlayerInfo = new PlayerTB();
        WeaponData newWeapon = new WeaponData();
        
        if (inputAddress.text.Length == 0 || inputName.text.Length == 0)
        {
            print("nope");
            return;
        }


        string body = JsonUtility.ToJson(playerTB);

        string newWeaponJsonData;

        HTTPClient.instance.POST("https://breadmore.azurewebsites.net/api/player_tb", body, delegate (string www)
        {
            print("new account!");
            newPlayerInfo = HandleNewAccountInfo(www);

            print(www);
            HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/weapon_tb/owner/" + newPlayerInfo.player_id,
            delegate (string result)
            {
                newWeapon = HandleNewAccountWeapon(result);
                newWeaponJsonData = JsonUtility.ToJson(newWeapon);

                print(newWeaponJsonData);
                //here
                setIPFS(newWeaponJsonData);
            });

        });

        newstartPanel.SetActive(false);

      
    }

    public IEnumerator setToken()
    {
        yield return StartCoroutine(PPC721Contract.SetToken("0x6A68CBa31DD3d3AC89a297DDFe0207BdE49Ed3c6", (Address, ex) =>
        {
            Debug.Log($"SetToken Contract Address: {Address}");
        }));
    }

    private async void setIPFS(string www)
    {
        metaDataWeapon.weaponData = JsonUtility.FromJson<WeaponData>(www);
        metaDataWeapon.image = "https://bafkreiezrpaxfumy7rbv4234krmboniyyuh5unnved2tf5btgfo7hy76iq.ipfs.nftstorage.link/";
        metaDataWeapon.name = "SweetP Weapon";
        print(metaDataWeapon.weaponData.weapon_id);

        string jsonData = JsonUtility.ToJson(metaDataWeapon);
        print("json: " + jsonData);
        NFTStorageUploadResponse uploadResponse = await ImplementNFTStorage.instance.NSC.UploadDataFromJsonHttpClient(jsonData);

        if (uploadResponse != null && uploadResponse.ok)
        {
            string uploadedCID = uploadResponse.value.cid;
            string ipfsUrl = "https://" + uploadedCID + ".ipfs.nftstorage.link/";
            Debug.Log("Uploaded CID: " + uploadedCID);
            StartCoroutine(MintNFT("0x30018fC76ca452C1522DD9C771017022df8b2321", ipfsUrl));
            // 이제 uploadedCID를 사용하여 IPFS 네트워크에서 데이터를 가져오거나 공유할 수 있습니다.
        }
        else
        {
            Debug.Log("Error uploading JSON data or retrieving CID.");
        }
    }

    public IEnumerator MintNFT(string recipient, string tokenURI)
    {
        yield return StartCoroutine(PPC721Contract.MintNFT(recipient , tokenURI ,(Address, ex) =>
        {
            Debug.Log($"MintNFT Contract Address: {Address}");
        }));
    }
}
