using System.Collections;
using UnityEngine;
using System;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;

public class FrequentlyUsed : MonoBehaviour
{
    // Start is called before the first frame update


    public static IEnumerator SendTransaction(SmartContractInteraction contractInstance, string recipientAddress, decimal ethValue, string data, Action<string, Exception> callback)
    {
        // Nounce 만들기
        string senderAddress = SmartContractInteraction.userAccount.Address;
        var nonceTask = contractInstance.web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(senderAddress);
        yield return new WaitUntil(() => nonceTask.IsCompleted);
        if (nonceTask.IsFaulted)
        {
            callback(null, nonceTask.Exception);
            yield break;
        }
        var nonce = new HexBigInteger(nonceTask.Result.Value);

        // Transaction Input 생성

        var transactionInput = new Nethereum.RPC.Eth.DTOs.TransactionInput()
        {
            From = senderAddress,
            To = recipientAddress, // Set this to the contract address
            Value = new HexBigInteger(Web3.Convert.ToWei(ethValue)),
            Gas = new HexBigInteger(3000000),
            GasPrice = new HexBigInteger(Nethereum.Util.UnitConversion.Convert.ToWei(2, 9)), // 2 Gwei
            Nonce = nonce,
            Data = data // 스마트 컨트렉트 함수에 제공할 파라미터 값들
        };

        // 트랜잭션 서명 및 전송
        yield return SignAndTransferTransaction(contractInstance.web3, transactionInput, (contractAddress, err) => {
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

    public static IEnumerator SignAndTransferTransaction(Web3 web3, TransactionInput transactionInput, Action<string, Exception> callback)
    {
        //트랜잭션 서명
        var signedTransactionTask = web3.TransactionManager.SignTransactionAsync(transactionInput);
        yield return new WaitUntil(() => signedTransactionTask.IsCompleted);
        if (signedTransactionTask.IsFaulted)
        {
            callback(null, signedTransactionTask.Exception);
        }
        else
        {
            var signedTransaction = signedTransactionTask.Result;
            // 서명된 트랜잭션 전송
            var sendTransactionTask = web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedTransaction);

            yield return new WaitUntil(() => sendTransactionTask.IsCompleted);
            if (sendTransactionTask.IsFaulted)
            {
                callback(null, sendTransactionTask.Exception);
            }
            // 트랜잭션이 제대로 Success 되었는지 반복해서 확인
            yield return CheckTransactionSuccess(web3, sendTransactionTask.Result, (transactionAddress, err) => {
                if (transactionAddress == null)
                {
                    callback(null, err);
                }
                else
                {
                    callback(transactionAddress, null);
                }
            });
        }
    }

    public static IEnumerator CheckTransactionSuccess(Web3 web3, string transactionAddress, Action<string, Exception> callback)
    {
        while (true)
        {
            var receiptTask = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionAddress);
            yield return new WaitUntil(() => receiptTask.IsCompleted);

            if (receiptTask.IsFaulted)
            {
                callback(null, receiptTask.Exception);
                yield break;
            }

            if (receiptTask.Result != null && receiptTask.Result.Status != null)
            {
                if (receiptTask.Result.Status.Value == 1) // Value 값이 1이면 Status가 Success 의미
                {
                    callback(transactionAddress, null);
                    yield break;
                }
                else
                {
                    callback(null, receiptTask.Exception);
                    Debug.Log(transactionAddress);
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.5f); // Wait for 0.2 seconds before checking again
        }
    }
}