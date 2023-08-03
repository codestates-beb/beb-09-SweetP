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
            Debug.Log(task.Result);
            decimal liquidityShare = (decimal)((double)task.Result / System.Math.Pow(10,18));
            callback(liquidityShare, null);
        }
    }

    public IEnumerator Swap(string senderAddress, decimal ethValue, decimal x, string tokenSymbol, Action<string, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("swap");

        // Nounce 만들기
        HexBigInteger nonce = null;
        yield return FrequentlyUsed.GetNounce(this.contractInstance.web3, senderAddress, (result, err)=>{
            if(result != null) {
                nonce = result;
            }else {
                Debug.Log(err);
            }
        });

        // Transaction Input 생성
        BigInteger tokenAmount = new BigInteger(x * (decimal)Math.Pow(10, 18));
        var data = function.GetData(tokenAmount, tokenSymbol);

        var transactionInput = new Nethereum.RPC.Eth.DTOs.TransactionInput() {
            From = senderAddress,
            To = this.contractInstance.contractAddress, // Set this to the contract address
            Value = new HexBigInteger(Web3.Convert.ToWei(ethValue)),
            Gas = new HexBigInteger(3000000),
            GasPrice = new HexBigInteger(Nethereum.Util.UnitConversion.Convert.ToWei(2, 9)), // 2 Gwei
            Nonce = nonce,
            Data = data
        };

        // 서명된 트랜잭션 생성
        var signedTransactionTask = this.contractInstance.web3.TransactionManager.SignTransactionAsync(transactionInput);
        yield return new WaitUntil(()=> signedTransactionTask.IsCompleted);
        if(signedTransactionTask.IsFaulted) {
            callback("", signedTransactionTask.Exception);
        } else {
            var signedTransaction = signedTransactionTask.Result;
            // 서명된 트랜잭션 전송
            var sendTransactionTask = this.contractInstance.web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
            
            yield return new WaitUntil(()=> sendTransactionTask.IsCompleted);
            if(sendTransactionTask.IsFaulted) {
                callback("", sendTransactionTask.Exception);
            } 
            // 트랜잭션이 제대로 Success 되었는지 반복해서 확인
            yield return FrequentlyUsed.CheckTransactionSuccess(this.contractInstance.web3, sendTransactionTask.Result, (transactionAddress, err)=>{
                if(transactionAddress == null) {
                    Debug.Log(err);
                }else {
                    callback(transactionAddress, null);
                }
            });
        }
    }


    public IEnumerator AddLiquidity(string fromAddress, decimal ethValue, Action<string, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("addLiquidity");
        var gas =  new HexBigInteger(500000);
        var limit = new HexBigInteger(3000000);
        var value = new HexBigInteger(Web3.Convert.ToWei(ethValue));
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

    public IEnumerator InitETH(string senderAddress, Action<string, Exception> callback) {
        var function = this.contractInstance.contract.GetFunction("initETH");
   
        var data = function.GetData();
        var nonceTask = this.contractInstance.web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(senderAddress);
        yield return new WaitUntil(() => nonceTask.IsCompleted);

        if(nonceTask.IsFaulted) {
            callback("", nonceTask.Exception);
            yield break;
        }
        var nonce = new HexBigInteger(nonceTask.Result.Value);
        var transactionInput = new Nethereum.RPC.Eth.DTOs.TransactionInput() {
            From = senderAddress,
            To = this.contractInstance.contractAddress, // Set this to the contract address
            Value = new HexBigInteger(Web3.Convert.ToWei(0.01)),
            Gas = new HexBigInteger(3000000),
            GasPrice = new HexBigInteger(2000000000),
            Data = data,
            Nonce = nonce,
        };
        var signedTransactionTask = this.contractInstance.web3.TransactionManager.SignTransactionAsync(transactionInput);
        
        yield return new WaitUntil(()=> signedTransactionTask.IsCompleted);
        
        if(signedTransactionTask.IsFaulted) {
            callback("", signedTransactionTask.Exception);
        } else {
            var signedTransaction = signedTransactionTask.Result;
            var sendTransactionTask = this.contractInstance.web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);
            
            yield return new WaitUntil(()=> sendTransactionTask.IsCompleted);

            if(sendTransactionTask.IsFaulted) {
                callback("", sendTransactionTask.Exception);
            } else {
                callback(sendTransactionTask.Result, null);
            }
        }
    }



    
}
