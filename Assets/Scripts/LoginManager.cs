using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

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
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
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
        if(inputLoginAddress.text.Length == 0)
        {
            return;
        }
        PlayerAddress = inputLoginAddress.text;
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/player_tb/address/"+PlayerAddress, delegate (string www)
        {
            HandleData(www);
            
            PlayerNameText.text = "Player Name : " + PlayerName;
            loginButton.SetActive(false);
            loginPanel.SetActive(false);
            newAccountButton.SetActive(false);
        });

        

    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
    }

    public void OpenNewstartPanel()
    {
        newstartPanel.SetActive(true);
    }

    public void NewStart()
    {
        PlayerTB playerTB = new PlayerTB();
        playerTB.player_address = inputAddress.text;
        playerTB.player_name = inputName.text;

        if(inputAddress.text.Length == 0 || inputName.text.Length == 0)
        {
            print("nope");
            return;
        }

        
        string body = JsonUtility.ToJson(playerTB);


        HTTPClient.instance.POST("https://breadmore.azurewebsites.net/api/player_tb", body, delegate (string www)
        {
            print("new account!");
        });

        newstartPanel.SetActive(false);

    }
}
