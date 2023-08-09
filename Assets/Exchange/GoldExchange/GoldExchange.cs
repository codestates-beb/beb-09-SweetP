using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.Json;

public class GoldExchange : MonoBehaviour
{
    public Transform mainUI;
    public string swapSymbol;
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    private Coroutine timerCoroutine;
    public bool toogleSwap;
    public TextMeshProUGUI inputXSymbolText;
    public TextMeshProUGUI inputYSymbolText;
    public TextMeshProUGUI inputXBalance;
    public TextMeshProUGUI inputYBalance;
    public int XBalance;
    public int YBalance;
    public Image goldUI;
    public Image PPCUI;
    public SweetpDex sweetpDex;

    public PPCTokenContract tokenContract;
    public Button swapButton;
    public TextMeshProUGUI swapButtonText;
    // Start is called before the first frame update
    void Awake()
    {
        inputX.onValueChanged.AddListener(delegate { ResetTimer();});
        inputX.contentType = TMP_InputField.ContentType.IntegerNumber; // 혹은 IntegerNumber
        swapSymbol = "Gold";
    }

    // Update is called once per frameEventSystem.current.SetSelectedGameObject(null);
    void ResetTimer(){ // 0.2초전에 타입시 GetSwapRatio 실행 방지를 위한 코드
        if(swapSymbol == "Gold") {
                if(int.Parse(inputX.text) > ItemManager.instance.itemData.player_gold) {
                    inputX.text = ItemManager.instance.itemData.player_gold.ToString();
                    // 입력 필드의 포커스를 제거
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        else if(swapSymbol == "PPC") {
            //print(int.Parse(inputX.text));
            print(sweetpDex.tokenBalance);
            if(int.Parse(inputX.text) > sweetpDex.tokenBalance)  {
                inputX.text = ((int)sweetpDex.tokenBalance).ToString();
                // 입력 필드의 포커스를 제거

                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        if(timerCoroutine != null) {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine() {
        yield return new WaitForSeconds(1f);
        if(string.IsNullOrEmpty(inputX.text)) {
            inputY.text = "";
        }else {
            StartCoroutine(GetSwapRatio());   
        }
    }

    public IEnumerator GetSwapRatio() {
        if(string.IsNullOrEmpty(inputX.text) ) {
            yield break;
        }
        if(swapSymbol == "Gold") {
            XBalance = RoundToThousand(int.Parse(inputX.text));
            inputX.text = XBalance.ToString();
            YBalance = XBalance / 1000;
            inputY.text = YBalance.ToString();
        }
        else if (swapSymbol == "PPC") {
            XBalance = int.Parse(inputX.text);
            inputX.text = XBalance.ToString();
            YBalance = XBalance * 1000;
            inputY.text = YBalance.ToString();
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public int RoundToThousand(int number) {
        return ((int) Math.Round((double) number / 1000)) * 1000;
    }
    void Update()
    {
        inputY.interactable = false;
        if(swapSymbol == "Gold") {
            inputXBalance.text = ItemManager.instance.itemData.player_gold.ToString("N0") + " Gold";
            inputYBalance.text = sweetpDex.tokenBalance.ToString("N0")  + " PPC";
            if(ItemManager.instance.itemData.player_gold < XBalance) {
                swapButton.interactable = false;
                swapButtonText.text = "InSufficent";
                swapButtonText.color = Color.red;
            }else {
                swapButton.interactable = true;
                swapButtonText.text = "Swap";
                swapButtonText.color = Color.white;
            }
            Mathf.Clamp(XBalance, 0, ItemManager.instance.itemData.player_gold);
        }
        else if (swapSymbol == "PPC") {
            inputXBalance.text = sweetpDex.tokenBalance.ToString("N0")  + " PPC";
            inputYBalance.text = ItemManager.instance.itemData.player_gold.ToString("N0")  + " Gold";
            if(sweetpDex.tokenBalance < XBalance) {
                swapButton.interactable = false;
                swapButtonText.text = "InSufficent";
                swapButtonText.color = Color.red;
            }else {
                swapButton.interactable = true;
                swapButtonText.text = "Swap";
                swapButtonText.color = Color.white;
            }
            Mathf.Clamp(XBalance, 0, (int)sweetpDex.tokenBalance);
        }
        

    }

    public void Enter() {
        mainUI.localPosition = Vector2.zero;
        inputX.text = "";
        // swapSymbol = "Gold";
        // toogleSwap = true;
        StartCoroutine(sweetpDex.SetInfo());
    }

    public void Exit() {
        mainUI.localPosition = Vector3.up * 80000;
    }

    public void ChangeSwapSymbol() //Swap 버튼 누를시 작동
    {
        StartCoroutine(sweetpDex.SetInfo());
        toogleSwap = !toogleSwap;
        if(toogleSwap) {
            swapSymbol = "Gold";
            inputXSymbolText.text = "Gold";
            inputYSymbolText.text = "PPC";
        }
        if(!toogleSwap) {
            swapSymbol = "PPC";
            inputXSymbolText.text = "PPC";
            inputYSymbolText.text = "Gold";

        }
        Vector3 tempPosition = goldUI.transform.position;
        goldUI.transform.position = PPCUI.transform.position;
        PPCUI.transform.position = tempPosition;

        string temp = inputX.text;
        inputX.text = inputY.text;
        inputY.text = temp;
    }

    public void OnSwapButtonClicked() {
        StartCoroutine(Swap());
    }

    private IEnumerator Swap() {
        bool isGotProblem = false;
        StartCoroutine(sweetpDex.SetInfo());
        HTTPClient.instance.StartSpinner();
        ItemData data = ItemManager.instance.itemData;
        if(swapSymbol == "Gold") {
            if(data.player_gold < XBalance) yield break;

            yield return tokenContract.transferFromSpecified("0x176feB0F409cecFd3362CD4C10fF730814368EfE", SmartContractInteraction.userAccount.Address, YBalance, (result, err) =>{
                    if(err != null) {
                        Debug.Log(err);
                        isGotProblem = true;
                        HTTPClient.instance.EndSpinner();
                    }
                    else {
                        inputX.text = "";
                        StartCoroutine(sweetpDex.SetInfo());
                        HTTPClient.instance.EndSpinner();
                    }
                });
             if(isGotProblem) yield break;
            data.player_gold -= XBalance;
            string jsonPayload = JsonUtility.ToJson(data);
            HTTPClient.instance.PUT($"https://breadmore.azurewebsites.net/api/Player_Data/{LoginManager.instance.PlayerID}", jsonPayload, (result)=>{
                if(string.IsNullOrEmpty(result)) {
                    Debug.Log("Failed to trade Gold");
                }
                HTTPClient.instance.EndSpinner();
                inputX.text = "";
                StartCoroutine(sweetpDex.SetInfo());
            });
        }
        else if(swapSymbol == "PPC") {
            yield return sweetpDex.tokenContract.transferFromSpecified(SmartContractInteraction.userAccount.Address, "0x176feB0F409cecFd3362CD4C10fF730814368EfE",  XBalance, (result, err) =>{
                if(err != null) {
                    Debug.Log(err);
                    isGotProblem = true;
                }                
            });
            if(isGotProblem) yield break;
            data.player_gold += YBalance;
            string jsonPayload = JsonUtility.ToJson(data);
            HTTPClient.instance.PUT($"https://breadmore.azurewebsites.net/api/Player_Data/{LoginManager.instance.PlayerID}", jsonPayload, (result)=>{
                if(string.IsNullOrEmpty(result)) {
                    Debug.Log("Failed to trade Gold");
                }
                HTTPClient.instance.EndSpinner();
                inputX.text = "";
                
            });
        }
        yield return sweetpDex.SetInfo();       
    }
}
