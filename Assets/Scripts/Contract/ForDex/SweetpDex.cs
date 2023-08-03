using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
// using System.IO;
// using System.Threading.Tasks;
// using Nethereum.Contracts;
// using Nethereum.Web3;
// using Newtonsoft.Json;

public class SweetpDex : MonoBehaviour
{
    public PPCTokenContract tokenContract;
    public SweetpDexContract dexContract;
    public DexSwap dexSwap;
    public LiquidityPool liquidityPool;
    public static string userAddress;
    public bool isOpenUI;

    public Transform swapUI;
    public Transform poolUI;
    public Transform daxMainUI;
    public decimal ethBalance;
    public decimal tokenBalance; 

    public decimal contractEthBalance;
    public decimal contractTokenBalance;
    public decimal liquidityShare;

    private int preNum;
    public GameObject progressCircle;

    // Start is called before the first frame update
    private void Awake()    
    {
        // userAddress = "0x30018fC76ca452C1522DD9C771017022df8b2321";
        tokenContract.Initialize();
        dexContract.Initialize();
        
    }
    void OnEnable(){
        tokenContract.Initialize();
        dexContract.Initialize();

    }
    public void ChangCategory(int num) {
        StartCoroutine(SetInfo());
        if(num == 0 && preNum != num) {
            swapUI.localPosition = Vector3.zero;
            poolUI.localPosition += Vector3.up * 80000;
            SwapResetInfo();

        } else if (num == 1 && preNum != num){
            swapUI.localPosition += Vector3.up * 80000;
            poolUI.localPosition = Vector3.zero;
            liquidityPool.swapSymbol = "ETH";
            liquidityPool.toogleSwap = true;
            liquidityPool.inputX.text = "";
            liquidityPool.inputXSymbolText.text = "ETH";
            liquidityPool.inputYSymbolText.text = "PPC";
        }
        preNum = num;
    }

    // Update is called once per frame
    void Update()
    {
        EnterOrExitDex();
        if(Input.GetKeyDown(KeyCode.M)) {
            Debug.Log("---------");
            StartCoroutine(dexContract.InitETH(SmartContractInteraction.userAccount.Address, (result, err)=>{
                if(err == null) {
                    Debug.Log(result);
                }else {
                    Debug.Log(err);
                }
            }));
        }
    }

    private void EnterOrExitDex() {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isOpenUI = !isOpenUI;
            if(isOpenUI){
                daxMainUI.localPosition = Vector3.zero;
                StartCoroutine(SetInfo());
                SwapResetInfo();
            }
            else if(!isOpenUI){
                daxMainUI.localPosition = Vector3.up * 80000;
            }
        }
    }

    public void SwapResetInfo() {
        dexSwap.swapRateText.text = "";
        dexSwap.swapButton.interactable = false;
        dexSwap.swapSymbol = "ETH";
        dexSwap.toogleSwap = true;
        dexSwap.inputX.text = "";
        dexSwap.inputXSymbolText.text = "ETH";
        dexSwap.inputYSymbolText.text = "PPC";
    }
    public IEnumerator SetInfo() // check out contract and My ETH,PPC amount, liquidity share
    {
        yield return StartCoroutine(dexContract.GetETHBalance((balance, ex)=>{
            if(ex == null) {
                contractEthBalance = balance;
            }else {
                Debug.Log(ex);
            }
        }));

        yield return StartCoroutine(dexContract.GetTokenBalance((balance, ex)=>{
            if(ex == null) {
                contractTokenBalance = balance;
            }
            else {
                Debug.Log(ex);
            }
            }
        ));
        
        yield return StartCoroutine(dexContract.GetMyLiquidityShare((balance, ex)=>{
            if(ex == null) {
                liquidityShare = balance;
            }
            else {
                Debug.Log(ex);
            }
        }));

        yield return StartCoroutine(tokenContract.BalanceOf(SmartContractInteraction.userAccount.Address, (balance, ex)=>{
            if(ex == null) {
                tokenBalance = balance;
            }
            else {
                Debug.Log(ex);
            }
        }));

        yield return StartCoroutine(tokenContract.GetBalance(SmartContractInteraction.userAccount.Address, (balance, ex)=>{
            if(ex == null) {
                ethBalance = balance;
            }
            else {
                Debug.Log(ex);
            }
        }));

       
    }
}
