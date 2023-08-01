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
    }


    public IEnumerator DepositETH(string fromAddress, decimal valueParam, int gasPriceParam) {
        var function = this.contractInstance.contract.GetFunction("depositETH");
        var gasPrice = new HexBigInteger(gasPriceParam);
        
        var value = new HexBigInteger(Web3.Convert.ToWei(valueParam));  // 5 Ether 전송
        var gasLimit = new HexBigInteger(600000); // 예시로 600,000을 사용함
        var task = function.SendTransactionAsync(fromAddress, gasPrice, gasLimit, value);
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.IsFaulted)
        {
            // 오류 처리
            Debug.LogError(task.Exception);
        }
        else
        {
            //  결과 발행된 트렌젝션 주소 출력
            var result = task.Result;
            Debug.Log("Success to execute depositETH function");
            Debug.Log("Transaction address : " + result);
        }
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
            decimal liquidityShare = (decimal)((double)task.Result / System.Math.Pow(10,18));
            callback(liquidityShare, null);
        }
    }

    public IEnumerator Swap(string fromAddress, decimal ethValue, decimal x, string tokenSymbol, Action<string, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("swap");
        var gas = new HexBigInteger(500000);
        var gasLimit = new HexBigInteger(3000000);
        var value = new HexBigInteger(Web3.Convert.ToWei(ethValue));  // 5 Ether 전송
        BigInteger tokenAmount = new BigInteger(x * (decimal)Math.Pow(10, 18));
        var task = function.SendTransactionAsync(fromAddress, gas, gasLimit, value, new object[] { tokenAmount, tokenSymbol });
        yield return new WaitUntil(()=>task.IsCompleted);
        if(task.IsFaulted) {
            callback("", task.Exception);
        } else {
            callback(task.Result, null);
        }
    }

    public IEnumerator AddLiquidity(string fromAddress, decimal valueParam, Action<string, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("addLiquidity");
        var gas =  new HexBigInteger(500000);
        var limit = new HexBigInteger(3000000);
        var value = new HexBigInteger(Web3.Convert.ToWei(valueParam));
        var task = function.SendTransactionAsync(fromAddress, gas, limit, value);
        yield return new WaitUntil(()=>task.IsCompleted);
        if(task.IsFaulted) {
            callback("", task.Exception);
        }else {
            callback(task.Result, null);
        }
    }

    public IEnumerator RemoveLiquidity(string fromAddress,  float valueParam, Action<string, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("removeLiquidity");
        var gas = new HexBigInteger(500000);
        var limit = new HexBigInteger(3000000);
        var value = new HexBigInteger(Web3.Convert.ToWei(valueParam));
        var task = function.SendTransactionAsync(fromAddress, gas, limit, value);
        yield return new WaitUntil(()=>task.IsCompleted);
        if(task.IsFaulted) {
            callback("", task.Exception);
        }else {
            callback(task.Result, null);
        }
    }
}
