using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RemoveLiquidity : MonoBehaviour, IPointerClickHandler
{
    public Transform mainUI;
    public Image slice;
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public TextMeshProUGUI inputXSymbolText;
    public TextMeshProUGUI inputYSymbolText;
    public TextMeshProUGUI ethTotalAmountText;
    public TextMeshProUGUI ppcTotalAmountText;
    public TextMeshProUGUI myEthAmountText;
    public TextMeshProUGUI myPpcAmountText;
    public TextMeshProUGUI liquidityShareText;

    public string swapSymbol;
    public bool toogleSwap;
    public SweetpDex sweetpDex;
    public decimal pairTokenAmount;
    private Coroutine timerCoroutine;
    public decimal removeAmount;
    public decimal myEthAmount;
    public decimal myPpcAmount;
    public Button removeButton;


    public void Awake() {
        swapSymbol = "ETH";
        toogleSwap = true;
        inputX.onValueChanged.AddListener(delegate { ResetTimer(); CheckInputValue();});
        inputX.contentType = TMP_InputField.ContentType.DecimalNumber; // 혹은 IntegerNumber
    }


    void ResetTimer(){ // x초전에 타입시 GetPairTokenAmount 실행 방지를 위한 코드
        if(timerCoroutine != null) {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    void CheckInputValue() {
        if(swapSymbol == "ETH") {
            if(StringToDecimal(inputX.text) > myEthAmount) {
                inputX.text = myEthAmount.ToString("N6");
                // 입력 필드의 포커스를 제거
                EventSystem.current.SetSelectedGameObject(null);

                // 입력 필드에 다시 포커스를 주면 커서 위치가 맨 오른쪽으로 이동
                // EventSystem.current.SetSelectedGameObject(inputX.gameObject);
            }
        }
        else if(swapSymbol == "PPC") {
            if(StringToDecimal(inputX.text) > myPpcAmount) {
                inputX.text = myPpcAmount.ToString("N2");
                // 입력 필드의 포커스를 제거
                EventSystem.current.SetSelectedGameObject(null);

                // 입력 필드에 다시 포커스를 주면 커서 위치가 맨 오른쪽으로 이동
                // EventSystem.current.SetSelectedGameObject(inputX.gameObject);
            }
        }

    }

    private IEnumerator TimerCoroutine() {
        yield return new WaitForSeconds(0.01f);
        if(string.IsNullOrEmpty(inputX.text)) {
            inputY.text = "";
        }else {
            StartCoroutine(GetPairTokenAmount());   
        }
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

    private void Update() {
        inputY.interactable = false;
        myEthAmount = sweetpDex.contractEthBalance * sweetpDex.liquidityShare;
        myPpcAmount = sweetpDex.contractTokenBalance * sweetpDex.liquidityShare;
        ethTotalAmountText.text = DexSwap.FormatDecimal(sweetpDex.contractEthBalance,6);
        ppcTotalAmountText.text = DexSwap.FormatDecimal(sweetpDex.contractTokenBalance,2);
        myEthAmountText.text = DexSwap.FormatDecimal(sweetpDex.contractEthBalance * sweetpDex.liquidityShare,6);
        myPpcAmountText.text = DexSwap.FormatDecimal(sweetpDex.contractTokenBalance * sweetpDex.liquidityShare,2);
        liquidityShareText.text = DexSwap.FormatDecimal(sweetpDex.liquidityShare * 100,4) + "%";
        if(swapSymbol == "ETH") {
            slice.fillAmount = (float)(StringToDecimal(inputX.text) / myEthAmount);
        }
        else if(swapSymbol == "PPC") {
            slice.fillAmount = (float)(StringToDecimal(inputX.text) / myPpcAmount);
        }
        if(string.IsNullOrEmpty(inputX.text)) {
            removeButton.interactable = false;
        }else {
            removeButton.interactable = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(slice.rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);

        // 이미지의 상단에서 클릭한 위치를 계산합니다.
        float clickedPositionInImage = slice.rectTransform.rect.height - (localPoint.y + slice.rectTransform.rect.height * 0.5f);

        // 클릭한 위치를 기반으로 fillAmount 값을 계산합니다.
        slice.fillAmount = clickedPositionInImage / slice.rectTransform.rect.height;
    }

    public void OnRemoveLiquidityButtonClicked() {
        StartCoroutine(RemoveLiquidityShare());
    }

    private IEnumerator RemoveLiquidityShare() {
        decimal ethValue = 0;
        if(swapSymbol == "ETH") {
            ethValue = StringToDecimal(inputX.text);
            if(slice.fillAmount == 1) { // 전액을 유동성 회수 할 때 solidity에서 에러뜨는 오류있음
                ethValue *= (decimal)0.999999f;
                Debug.Log("eee");
            }
        }else if (swapSymbol == "PPC") {
            ethValue = StringToDecimal(inputY.text);
            if(slice.fillAmount == 1) {
                ethValue *= (decimal)0.999999f;
            }
        }
        
        sweetpDex.progressCircle.SetActive(true);
        yield return sweetpDex.dexContract.RemoveLiquidity(ethValue, (result, err)=>{
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

    public void Enter() {
        mainUI.localPosition = Vector3.zero;
        StartCoroutine(sweetpDex.SetInfo());
    }

    public void Exit() {
        mainUI.localPosition += Vector3.up * 80000;
    }
}
