using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DexSwap : MonoBehaviour 
{
    public SweetpDex sweetpDex;
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public string swapSymbol;
    public bool toogleSwap;
    private Coroutine timerCoroutine;
    public decimal SwapAmount;

    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI swapRateText;
    public decimal swapRatio;
    public TextMeshProUGUI inputXSymbolText;
    public TextMeshProUGUI inputYSymbolText;
    public decimal slipageRate;
    public TextMeshProUGUI slipageText;
    public TextMeshProUGUI swapText;
    public Button swapButton;
    public Button toggleButton;


    
    // Start is called before the first frame update
    private void Awake() {
        inputX.onValueChanged.AddListener(delegate { ResetTimer();});
        inputX.contentType = TMP_InputField.ContentType.DecimalNumber; // 혹은 IntegerNumber
        swapSymbol = "ETH";
        inputXSymbolText.text = "ETH";
        inputYSymbolText.text = "PPC";
        inputY.interactable = false; 
        toogleSwap = true;    
    }

    void Update()
    {
        UpdateSwapButton();
        ChangeInputXToRed();
        if(swapSymbol == "ETH") {
            balanceText.text = sweetpDex.ethBalance.ToString("N6") + " ETH";
        }else if(swapSymbol == "PPC") {
            balanceText.text = sweetpDex.tokenBalance.ToString("N2") + " PPC";
        }
        if(sweetpDex.isLoading) {
            inputX.interactable = false;
            toggleButton.interactable = false;
        }else {
            inputX.interactable = true;
            toggleButton.interactable = true;
        }
    }


    private void UpdateSwapButton() {
        if(StringToDecimal(inputX.text) == 0 || sweetpDex.isLoading) {
            swapButton.interactable = false;
            swapText.text = "Swap";
            swapText.color = Color.white;
            return;
        }
        if(swapSymbol == "ETH") {
            balanceText.text = FormatDecimal(sweetpDex.ethBalance, 2) + " ETH";
            if(sweetpDex.ethBalance < StringToDecimal(inputX.text)) {
                swapText.text = "InSufficent";
                swapText.color = Color.red;
                swapButton.interactable = false;
            }
            else {
                swapText.text = "Swap";
                swapText.color = Color.white;
                swapButton.interactable = true;
            }
        } else if(swapSymbol == "PPC") {
            balanceText.text = FormatDecimal(sweetpDex.tokenBalance, 6) + " PPC";
            if(sweetpDex.tokenBalance < StringToDecimal(inputX.text)) {
                swapText.text = "InSufficent";
                swapText.color = Color.red;
                swapButton.interactable = false;
            }
            else {
                swapText.text = "Swap";
                swapText.color = Color.white;
                swapButton.interactable = true;
            }
        }
    }
    void ResetTimer(){ // 1초전에 타입시 GetSwapRatio 실행 방지를 위한 코드
        if(timerCoroutine != null) {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine() {
        yield return new WaitForSeconds(0.2f);
        if(string.IsNullOrEmpty(inputX.text)) {
            swapRateText.text = "";
            inputY.text = "";
            slipageText.text = "";
        }else {
            StartCoroutine(GetSwapRatio());   
        }
    }

    public IEnumerator GetSwapRatio() {
        if(string.IsNullOrEmpty(inputX.text) ) {
            yield break;
        }
        if(StringToDecimal(inputX.text) == 0) {
            yield break;
        }
       yield return StartCoroutine(sweetpDex.SetInfo());

        // Update the local variables
        decimal ethBalance = sweetpDex.contractEthBalance;
        decimal tokenBalance = sweetpDex.contractTokenBalance;
        
        if( ethBalance <= 0 || tokenBalance <= 0) {
            SwapAmount = 0;
        }
        else {
            var k = ethBalance * tokenBalance;
            if(swapSymbol == "ETH") {
                SwapAmount = (decimal)((k/ethBalance) - (k/(ethBalance + StringToDecimal(inputX.text))));
                swapRatio = SwapAmount/StringToDecimal(inputX.text);
                swapRateText.text = "1ETH :" + FormatDecimal(swapRatio, 2) + " PPC";
                inputY.text = FormatDecimal(SwapAmount,2);
            }
            if(swapSymbol == "PPC") {
                SwapAmount = (decimal)((k/tokenBalance) - (k/(tokenBalance + StringToDecimal(inputX.text))));
                swapRatio = SwapAmount/StringToDecimal(inputX.text);
                swapRateText.text = "1PPC :" + FormatDecimal(swapRatio, 10) + " ETH";
                inputY.text = FormatDecimal(SwapAmount,6);    
            }
            CalculateSlipageRate();
            slipageText.text = FormatDecimal(slipageRate * 100,4) + "%";
           if(slipageRate >= (decimal)0.05) {
                slipageText.color = Color.red;
                swapRateText.color = Color.red;
            }
            else if(slipageRate >= (decimal)0.01) {
                slipageText.color = Color.yellow;
                swapRateText.color = Color.yellow;
            }
            else {
                slipageText.color = Color.green;
                swapRateText.color = Color.green;
            }
        }
    }

    public void OnSwapButtonClicked() {
        StartCoroutine(Swap());
    }
    public IEnumerator Swap() {
        sweetpDex.progressCircle.SetActive(true);
        sweetpDex.isLoading = true;
        if(swapSymbol == "ETH") {
            yield return sweetpDex.dexContract.Swap(StringToDecimal(inputX.text), 1, swapSymbol, (result, err)=>{
                if(string.IsNullOrEmpty(result)) {
                    Debug.Log(err);
                    sweetpDex.progressCircle.SetActive(false);
                    sweetpDex.isLoading = false;
                }else {
                    StartCoroutine(sweetpDex.SetInfo());
                    inputX.text = "";
                    sweetpDex.progressCircle.SetActive(false);
                    sweetpDex.isLoading = false;
                }
            });
        }
        else if (swapSymbol == "PPC") {
            yield return sweetpDex.tokenContract.Approve(sweetpDex.dexContract.contractInstance.contractAddress, StringToDecimal(inputX.text), (result, err)=>{
                if(string.IsNullOrEmpty(result)) {
                    Debug.Log(err);
                    sweetpDex.progressCircle.SetActive(false);
                    sweetpDex.isLoading = false;
                }
            });
            yield return sweetpDex.dexContract.Swap(0, StringToDecimal(inputX.text), swapSymbol, (result, err)=>{
                if(string.IsNullOrEmpty(result)) {
                    Debug.Log(err);
                    sweetpDex.progressCircle.SetActive(false);
                    sweetpDex.isLoading = false;
                } else {
                    StartCoroutine(sweetpDex.SetInfo());
                    inputX.text = "";
                    sweetpDex.progressCircle.SetActive(false);
                    sweetpDex.isLoading = false;
                }
            });
        }
        
    }

    private decimal StringToDecimal(string str) {
        decimal inputValue;
        bool success = decimal.TryParse(str, out inputValue);
        if(success) {
               return inputValue;
        }else {
            return 0;
        }
        
    }
    public void CalculateSlipageRate() {
        decimal actualRate = 0;
        if(swapSymbol == "ETH") {
            actualRate = sweetpDex.contractTokenBalance/sweetpDex.contractEthBalance;
        }
        else if(swapSymbol == "PPC") {
            actualRate = sweetpDex.contractEthBalance/sweetpDex.contractTokenBalance;
        }
        else {
            Debug.Log("The SwapSybol is neither ETH nor PPC");
        }
        slipageRate = ((actualRate - swapRatio)/actualRate);
    }

    static public string FormatDecimal(decimal value, int digitNumber)
{
    // 소수점 이하 8자리까지 표시하되, 필요없는 0은 제거
    string str = value.ToString("F" + digitNumber.ToString()).TrimEnd('0');

    // 소수점 이하가 모두 0이면 소수점 제거
    if (str.EndsWith("."))
    {
        str = str.TrimEnd('.');
    }

    // 세 자리마다 콤마 추가
    int dotIndex = str.IndexOf('.');
    if (dotIndex != -1)
    {
        // 소수점 앞 부분에 콤마 추가
        string wholePart = string.Format("{0:N0}", long.Parse(str.Substring(0, dotIndex)));
        // 소수점 이하 부분
        string fractionPart = str.Substring(dotIndex);

        return wholePart + fractionPart;
    }
    else
    {
        // 소수점 없는 경우
        return string.Format("{0:N0}", long.Parse(str));
    }
}

    // Update is called once per frame

    private void ChangeInputXToRed() { // 계좌 잔액보다 많이 입력시 텍스트색이 레드로 변환
        
        
        if(swapSymbol == "ETH" && StringToDecimal(inputX.text) > sweetpDex.ethBalance) {
            inputX.textComponent.color = Color.red;
        }
        else if (swapSymbol == "PPC" && StringToDecimal(inputX.text) > sweetpDex.tokenBalance) {
            inputX.textComponent.color = Color.red;
        }
        else {
            inputX.textComponent.color = Color.white;
        }
    }

    public void ChangeSwapSymbol() //symbol toogle 버튼 누를시 작동
    {
        toogleSwap = !toogleSwap;
        if(toogleSwap) {
            swapSymbol = "ETH";
            inputXSymbolText.text = "ETH";
            inputYSymbolText.text = "PPC";
        }
        if(!toogleSwap) {
            swapSymbol = "PPC";
            inputXSymbolText.text = "PPC";
            inputYSymbolText.text = "ETH";

        }
        inputX.text = inputY.text;
        StartCoroutine(sweetpDex.SetInfo());
        StartCoroutine(GetSwapRatio());
    }


}
