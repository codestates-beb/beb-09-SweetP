using System.Collections;
using UnityEngine;
using Nethereum.Web3;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using System;

public class SweetpDexContract : MonoBehaviour{
    
    public SmartContractInteraction contractInstance;

    void Awake() {
        contractInstance = gameObject.AddComponent<SmartContractInteraction>();
    }
    public void Initialize() 
    {
        contractInstance.Init("SweetpDexContract.json");
        Debug.Log(contractInstance.abi);
    }

    public IEnumerator GetTokenBalance(Action<decimal, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("getTokenBalance");
        var task = function.CallAsync<BigInteger>();
        yield return new WaitUntil(()=>task.IsCompleted);
        if(task.IsFaulted) {
            callback(0, task.Exception);
        }
        else {
            var tokenBalance = (decimal)((double)task.Result/ System.Math.Pow(10, 18));
            callback(tokenBalance, null);
        }
    }

    public IEnumerator GetETHBalance(Action<decimal, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("getETHBalance");
        var task = function.CallAsync<BigInteger>();
        yield return new WaitUntil(()=>task.IsCompleted);
        if(task.IsFaulted) {
            callback(0, task.Exception);
        }
        else {
            var ethBalance = (decimal)((double)task.Result/ System.Math.Pow(10, 18));
            callback(ethBalance, null);
        }
    }

    public IEnumerator GetMyLiquidityShare(Action<decimal, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("getMyLiquidityShare");
        var task = function.CallAsync<BigInteger>();
        yield return new WaitUntil(()=>task.IsCompleted);
        if(task.IsFaulted) {
            callback(0, task.Exception);
        } else {
            Debug.Log(task.Result);
            decimal liquidityShare = (decimal)((double)task.Result / System.Math.Pow(10,18));
            callback(liquidityShare, null);
        }
    }

    public IEnumerator Swap(decimal ethValue, decimal x, string tokenSymbol, Action<string, Exception> callback) {

        var function = this.contractInstance.contract.GetFunction("swap");
        BigInteger tokenAmount = new BigInteger(x * (decimal)Math.Pow(10, 18));
        string data = function.GetData(tokenAmount, tokenSymbol);

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err)=>{
            if(contractAddress == null) {
                callback(null, err);
            }else {
                callback(contractAddress, null);
            }
        });
        
        
    }


    public IEnumerator AddLiquidity(decimal ethValue, Action<string, Exception> callback) {
        
        var function = this.contractInstance.contract.GetFunction("addLiquidity");
        string data = function.GetData();

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err)=>{
            if(contractAddress == null) {
                callback(null, err);
            }else {
                callback(contractAddress, null);
            }
        });
    }

    public IEnumerator RemoveLiquidity(string senderAddress, decimal ethValue, Action<string, Exception> callback) {

        var function = this.contractInstance.contract.GetFunction("removeLiquidity");
        string data = function.GetData();

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err)=>{
            if(contractAddress == null) {
                callback(null, err);
            }else {
                callback(contractAddress, null);
            }
        });
    }

    public IEnumerator InitETH(string senderAddress, decimal ethValue, Action<string, Exception> callback) {
       var function = this.contractInstance.contract.GetFunction("initETH");
        string data = function.GetData();

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err)=>{
            if(contractAddress == null) {
                callback(null, err);
            }else {
                callback(contractAddress, null);
            }
        });
    }
}
