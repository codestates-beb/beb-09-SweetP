using System.Collections;
using UnityEngine;
using System;
using Nethereum.Hex.HexTypes;


public class PPC721Test : MonoBehaviour
{
    public SmartContractInteraction contractInstance;

    void Awake()
    {
        contractInstance = gameObject.AddComponent<SmartContractInteraction>();
    }

    public void Initialize()
    {
        contractInstance.Init("PPC721 Contract.json");
    }

    public async void MintNFT(string recipient, string tokenURI, decimal nftPrice, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("mintNFT");

        var transactionInput = function.CreateTransactionInput(recipient, tokenURI, nftPrice);
        // ... (트랜잭션 옵션 설정)

        try
        {
            var transactionHash = await function.SendTransactionAsync(transactionInput);
            Debug.Log("MintNFT completed with transaction hash: " + transactionHash);
            callback(transactionHash, null);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            callback("", ex);
        }
    }
}
