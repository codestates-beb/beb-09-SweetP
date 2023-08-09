using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    // Start is called before the first frame update
    void Awake()
    {
        inputX.onValueChanged.AddListener(delegate { ResetTimer();});
        inputX.contentType = TMP_InputField.ContentType.IntegerNumber; // 혹은 IntegerNumber
    }

    // Update is called once per frameEventSystem.current.SetSelectedGameObject(null);
    void ResetTimer(){ // 0.2초전에 타입시 GetSwapRatio 실행 방지를 위한 코드
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
            inputXBalance.text = ItemManager.instance.itemData.player_id.ToString("N0") + " Gold";
            inputYBalance.text = sweetpDex.tokenBalance.ToString("N0")  + " PPC";
        }
        else if (swapSymbol == "PPC") {
            inputXBalance.text = sweetpDex.tokenBalance.ToString("N0")  + " PPC";
            inputYBalance.text = ItemManager.instance.itemData.player_id.ToString("N0")  + " Gold";
        }
    }

    public void Enter() {
        mainUI.localPosition = Vector2.zero;
        swapSymbol = "Gold";
        toogleSwap = true;
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
        StartCoroutine(sweetpDex.SetInfo());
        yield return null;
    }
}
