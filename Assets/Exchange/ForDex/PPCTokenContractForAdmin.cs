using System.Collections;
using UnityEngine;
using System.Numerics;
using System;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;


public class PPCTokenContractForAdmin: MonoBehaviour{

    public SmartContractInteraction contractInstance;
    public decimal tokenBalance;
    public decimal ethBalance;
    void Awake(){
        contractInstance = gameObject.AddComponent<SmartContractInteraction>();
        Initialize(); 
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
            Debug.Log(contractAddress);
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


}
