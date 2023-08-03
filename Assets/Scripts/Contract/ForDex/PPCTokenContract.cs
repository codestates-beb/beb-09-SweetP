using System.Collections;
using UnityEngine;
using System.Numerics;
using System;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;


public class PPCTokenContract: MonoBehaviour{
    // Action<IEnumerator> startCoroutine;
    //0xC30867420c3E12287F3ab53B80849F527d4D2456
    public SmartContractInteraction contractInstance;
    public decimal tokenBalance;
    public decimal ethBalance;
    
    void Awake(){
        contractInstance = gameObject.AddComponent<SmartContractInteraction>();
    }

    public void Initialize() 
    {
        
        contractInstance.Init("PPCTokenContract.json");
    }

    public IEnumerator BalanceOf(string account, Action<decimal, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("balanceOf");
        var task = function.CallAsync<System.Numerics.BigInteger>(account);
        yield return new WaitUntil(()=> task.IsCompleted);
        if (task.IsFaulted) {
            Debug.LogError(task.Exception);
        }
        else {
            try {
                var tokenBalance = ((decimal)task.Result)/ (decimal)System.Math.Pow(10, 18);
                callback(tokenBalance, null);
            }
            catch (System.OverflowException ex) {
                callback(0, ex);
            }
        }
    }
        public IEnumerator GetBalance(string account, Action<decimal, Exception> callback) 
    {
        var function = this.contractInstance.web3.Eth.GetBalance;
        var task = function.SendRequestAsync(account);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) 
        {
            Debug.LogError(task.Exception);
        }
        else 
        {
            try 
            {
                ethBalance = Web3.Convert.FromWei(task.Result.Value);
                callback(ethBalance, null);
            }
            catch (System.Exception ex) 
            {
                callback(0, ex);
            }
            
        }
    }

    public IEnumerator Transfer(string fromAddress, string recipient, decimal amount) 
    {
        var function = this.contractInstance.contract.GetFunction("transfer");
        var weiAmount = BigInteger.Parse((amount * (decimal)Math.Pow(10, 18)).ToString("0"));
        var transactionInput = function.CreateTransactionInput(fromAddress, new object[] {recipient, weiAmount});
        transactionInput.Gas = new HexBigInteger(new BigInteger(3000000)); // 예시로 3000000을 설정했습니다.
        
        var task = function.SendTransactionAsync(transactionInput);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) 
        {
            Debug.LogError(task.Exception);
        }
        else 
        {
            Debug.Log("Transfer completed with transaction hash: " + task.Result);
        }
    }

    public IEnumerator Approve(string fromAddress, string spender, decimal amount, Action<string, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("approve");
        BigInteger amountWei = BigInteger.Multiply(new BigInteger(amount), BigInteger.Pow(10, 18));
        // var value = new HexBigInteger(valueParam * (decimal)Math.Pow(10, 18));  // 5 Ether 전송
        var estimateGasTask = function.EstimateGasAsync(fromAddress, null, null, spender, amountWei);
        yield return new WaitUntil(()=>estimateGasTask.IsCompleted);
        if(estimateGasTask.IsFaulted) {
            callback("", estimateGasTask.Exception);
            yield break;
        }
        var gas = estimateGasTask.Result;
        Debug.Log("EstimatedGas is : " + gas);
        var gasLimit = new HexBigInteger(3000000);
        var task = function.SendTransactionAsync(fromAddress, gas, gasLimit, new object[] {spender, amountWei});
        yield return new WaitUntil(()=>task.IsCompleted);
        if(task.IsFaulted) {
            callback("", task.Exception);
        }else {
            callback(task.Result, null);
        }
    }




}
