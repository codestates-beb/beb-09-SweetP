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

    [HideInInspector]
    public string PlayerAddress;
    [HideInInspector]
    public int PlayerID;
    [HideInInspector]
    public string PlayerName;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        //test user
        PlayerAddress = "lepo";
    }

    private void HandleData(string jsonData)
    {
        PlayerTB playerTB = JsonUtility.FromJson<PlayerTB>(jsonData);

        PlayerID = playerTB.player_id;
        PlayerName = playerTB.player_name;
    }
    
    public void Login()
    {
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/player_tb/address/"+PlayerAddress, delegate (string www)
        {
            HandleData(www);
            print(PlayerID);

            PlayerNameText.text = "Player Name : " + PlayerName;
        });
    }
}
