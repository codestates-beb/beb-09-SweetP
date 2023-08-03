using System.Collections;
using UnityEngine;
using System;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;

public class FrequentlyUsed : MonoBehaviour
{
    // Start is called before the first frame update
    public static IEnumerator GetNounce(Web3 web3, string senderAddress, Action<HexBigInteger, Exception> callback) {
        var nonceTask = web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(senderAddress);
        yield return new WaitUntil(() => nonceTask.IsCompleted);
        if(nonceTask.IsFaulted) {
            callback(null, nonceTask.Exception);
            yield break;
        }
        var nonce = new HexBigInteger(nonceTask.Result.Value);
        callback(nonce, null);
    }

    public static IEnumerator CheckTransactionSuccess(Web3 web3, string transactionAddress, Action<string, Exception> callback) {
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
                        callback(null, new Exception("Transaction failed"));
                        yield break;
                    }
                }

                yield return new WaitForSeconds(0.5f); // Wait for 0.2 seconds before checking again
            }
    }
}
