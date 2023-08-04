using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LiquidityPool : MonoBehaviour
{
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public TextMeshProUGUI inputXSymbolText;
    public TextMeshProUGUI inputYSymbolText;
    public TextMeshProUGUI inputXBalanceText;
    public TextMeshProUGUI inputYBalanceText;
    public TextMeshProUGUI ethTotalAmountText;
    public TextMeshProUGUI ppcTotalAmountText;
    public TextMeshProUGUI myEthAmountText;
    public TextMeshProUGUI myPpcAmountText;
    public TextMeshProUGUI liquidityShareText;
    public SweetpDex sweetpDex;

    public string swapSymbol;
    public bool toogleSwap;
    private Coroutine timerCoroutine;

    private decimal pairTokenAmount;

    public Button addButton;
    public TextMeshProUGUI addText;


    // Start is called before the first frame update
    void Awake()
    {
        inputX.onValueChanged.AddListener(delegate { ResetTimer();});
        inputX.contentType = TMP_InputField.ContentType.DecimalNumber; // 혹은 IntegerNumber
    }
    void ResetTimer(){ // 1초전에 타입시 GetPairTokenAmount 실행 방지를 위한 코드
        if(timerCoroutine != null) {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine() {
        yield return new WaitForSeconds(0.2f);
        if(string.IsNullOrEmpty(inputX.text)) {
            inputY.text = "";
        }else {
            StartCoroutine(GetPairTokenAmount());   
        }
    }

    private IEnumerator GetPairTokenAmount() {
        if(string.IsNullOrEmpty(inputX.text) ) {
            yield break;
        }
        decimal inputValue = StringToDecimal(inputX.text);
        if(inputValue == 0) {
            yield break;
        }

        yield return StartCoroutine(sweetpDex.SetInfo());
        

        // Update the local variables
        decimal ethBalance = sweetpDex.contractEthBalance;
        decimal tokenBalance = sweetpDex.contractTokenBalance;

        if( ethBalance <= 0 || tokenBalance <= 0) {
            pairTokenAmount = 0;
        }
        else {
            if(swapSymbol == "ETH") {
                pairTokenAmount = tokenBalance / ethBalance * inputValue;
                inputY.text = DexSwap.FormatDecimal(pairTokenAmount,2);
            }
            if(swapSymbol == "PPC") {
                pairTokenAmount = ethBalance  / tokenBalance * inputValue;
                inputY.text = DexSwap.FormatDecimal(pairTokenAmount,6);
                
            }
            
            
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

      private void ChangeInputToRed() { // 계좌 잔액보다 많이 입력시 텍스트색이 레드로 변환
      var isOver1 = false;
      var isOver2 = false;
        if(swapSymbol == "ETH" ) {
            if(StringToDecimal(inputX.text) > sweetpDex.ethBalance) {
                inputX.textComponent.color = Color.red;
                addText.text = "Insufficent";
                addText.color = Color.red;
                isOver1 = true;
            }
            else {
                inputX.textComponent.color = Color.white;
                isOver1 = false;
            }
            if(StringToDecimal(inputY.text) > sweetpDex.tokenBalance) {
                inputY.textComponent.color = Color.red;
                addText.text = "Insufficent";
                addText.color = Color.red;
                isOver2 = true;
            }
            else {
                inputY.textComponent.color = Color.white;
                isOver2 = false;
            }
            if(!isOver1 && !isOver2){
                addText.text = "Add";
                addText.color = Color.white;
            }
        }
        else if(swapSymbol == "PPC" ) {
            if(StringToDecimal(inputX.text) > sweetpDex.tokenBalance) {
                inputX.textComponent.color = Color.red;
                addText.text = "Insufficent";
                addText.color = Color.red;
                isOver1 = true;
            }
            else {
                inputX.textComponent.color = Color.white;
                isOver1 = false;
            }
            if(StringToDecimal(inputY.text) > sweetpDex.ethBalance) {
                inputY.textComponent.color = Color.red;
                addText.text = "Insufficent";
                addText.color = Color.red;
                isOver2 = true;
            }
            else {
                inputY.textComponent.color = Color.white;
                isOver2 = false;
            }
            if(!isOver1 && !isOver2){
                addText.text = "Add";
                addText.color = Color.white;
            }
        }
        
    }


    // Update is called once per frame
    void Update()
    {

        if(swapSymbol == "ETH") {
            inputXBalanceText.text = DexSwap.FormatDecimal(sweetpDex.ethBalance,6) + " ETH";
            inputYBalanceText.text = DexSwap.FormatDecimal(sweetpDex.tokenBalance,2) + " PPC";
        }else if (swapSymbol == "PPC"){
            inputXBalanceText.text = DexSwap.FormatDecimal(sweetpDex.tokenBalance,2) + " PPC";
            inputYBalanceText.text = DexSwap.FormatDecimal(sweetpDex.ethBalance,6) + " ETH";
        }

        ethTotalAmountText.text = DexSwap.FormatDecimal(sweetpDex.contractEthBalance,6);
        ppcTotalAmountText.text = DexSwap.FormatDecimal(sweetpDex.contractTokenBalance,2);
        myEthAmountText.text = DexSwap.FormatDecimal(sweetpDex.contractEthBalance * sweetpDex.liquidityShare,6);
        myPpcAmountText.text = DexSwap.FormatDecimal(sweetpDex.contractTokenBalance * sweetpDex.liquidityShare,2);
        liquidityShareText.text = DexSwap.FormatDecimal(sweetpDex.liquidityShare * 100,4) + "%";
        ChangeInputToRed();
        if(inputX.textComponent.color == Color.red || inputY.textComponent.color == Color.red || StringToDecimal(inputX.text) == 0) {
            addButton.interactable = false;
        } else {
            addButton.interactable = true;
        }
        inputY.interactable = false;

    }

    

    public void ChangeSwapSymbol() //Swap 버튼 누를시 작동
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
        string temp = inputX.text;
        inputX.text = inputY.text;
        inputY.text = temp;

        StartCoroutine(GetPairTokenAmount());
    }

    public void OnAddLiquidityButtonClicked() {
        StartCoroutine(AddLiquidity());
    }

    private IEnumerator AddLiquidity() {
        decimal ethValue = 0;
        decimal tokenValue = 0;
        if(swapSymbol == "ETH") {
            ethValue = StringToDecimal(inputX.text);
            tokenValue = StringToDecimal(inputY.text);
        }else if (swapSymbol == "PPC") {
            ethValue = StringToDecimal(inputY.text);
            tokenValue = StringToDecimal(inputX.text);
        }
        sweetpDex.progressCircle.SetActive(true);
        yield return sweetpDex.tokenContract.Approve(sweetpDex.dexContract.contractInstance.contractAddress, tokenValue * (decimal) 1.001, (result, err)=>{
                if(string.IsNullOrEmpty(result)) {
                    Debug.Log(err);
                    sweetpDex.progressCircle.SetActive(false);
                }
            });
        yield return sweetpDex.dexContract.AddLiquidity(ethValue, (result, err)=>{
            if (string.IsNullOrEmpty(result)) {
                Debug.Log(err);
                sweetpDex.progressCircle.SetActive(false);
            }
            else {
                inputX.text = "";
                sweetpDex.progressCircle.SetActive(false);
                StartCoroutine(sweetpDex.SetInfo());
            }
        });
    }
}
