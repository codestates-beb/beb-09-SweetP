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

    public IEnumerator BalanceOfNo(string account, Action<decimal, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("balanceOf");
        var task = function.CallAsync<System.Numerics.BigInteger>(account);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            try
            {
                var tokenBalance = ((decimal)task.Result);
                callback(tokenBalance, null);
            }
            catch (System.OverflowException ex)
            {
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

    public IEnumerator Transfer(string recipient, decimal amount, Action<string, Exception> callback) 
    {
        var function = this.contractInstance.contract.GetFunction("transfer");
        BigInteger tokenAmount = new BigInteger(amount * (decimal)Math.Pow(10, 18));
        string data = function.GetData(recipient, tokenAmount);

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, 0, data, (contractAddress, err)=>{
            if(contractAddress == null) {
                callback(null, err);
            }else {
                callback(contractAddress, null);
            }
        });
        
    }

    public IEnumerator TransferNo(string recipient, decimal amount, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("transfer");
        BigInteger tokenAmount = new BigInteger(amount);
        string data = function.GetData(recipient, tokenAmount);

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, 0, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                callback(null, err);
            }
            else
            {
                callback(contractAddress, null);
            }
        });

    }

    public IEnumerator Approve(string spender, decimal amount, Action<string, Exception> callback) {
         var function = this.contractInstance.contract.GetFunction("approve");
        BigInteger tokenAmount = new BigInteger(amount * (decimal)Math.Pow(10, 18));
        string data = function.GetData(spender, tokenAmount);

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, 0, data, (contractAddress, err)=>{
            if(contractAddress == null) {
                callback(null, err);
            }else {
                callback(contractAddress, null);
            }
        });
    }

    public IEnumerator ApproveNo(string spender, decimal amount, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("approve");
        BigInteger tokenAmount = new BigInteger(amount);
        string data = function.GetData(spender, tokenAmount);

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, 0, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                callback(null, err);
            }
            else
            {
                callback(contractAddress, null);
            }
        });
    }

    public IEnumerator transferFromSpecified(string sender, string recipient, decimal amount, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("transferFromSpecified");
        BigInteger tokenAmount = new BigInteger(amount * (decimal)Math.Pow(10, 18));
        string data = function.GetData(sender, recipient, tokenAmount);

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, 0, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                callback(null, err);
            }
            else
            {
                callback(contractAddress, null);
            }
        });

    }
    public IEnumerator transferFromSpecifiedNo(string sender, string recipient, decimal amount, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("transferFromSpecified");
        BigInteger tokenAmount = new BigInteger(amount);
        string data = function.GetData(sender, recipient, tokenAmount);

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, 0, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                callback(null, err);
            }
            else
            {
                callback(contractAddress, null);
            }
        });

    }




}
