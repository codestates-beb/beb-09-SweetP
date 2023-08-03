using UnityEngine;
using Nethereum.Web3;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections;


public class PPC721Contract : MonoBehaviour
{
    public SmartContractInteraction contractInstance;

    void Awake()
    {
        contractInstance = gameObject.AddComponent<SmartContractInteraction>();
        contractInstance.Init("PPC721Contract.json");
    }
    /*
    public void Initialize()
    {
        contractInstance.Init("PPC721Contract.json");
    }
    */


    
    public IEnumerator MintNFT()
    {


        var function = this.contractInstance.contract.GetFunction("mintNFT");


        // Transaction Input 생성
        string recipient = "0xE503081665f268c99ff22F45Df5FC8f3A21Ef0C8";
        string tokenURI = "https://gateway.pinata.cloud/ipfs/QmP25FavCnPjQpuvM9noxxfZNNRKW6cmUYsEg3LwSJ22gm/1.json";
        System.Numerics.BigInteger nftPrice = new System.Numerics.BigInteger(1);
        var data = function.GetData(recipient, tokenURI, nftPrice);
        decimal ethValue = 0;
        string senderAddress = "0xE503081665f268c99ff22F45Df5FC8f3A21Ef0C8";

        // 스마트 컨트랙트의 메서드에 전달할 트랜잭션 입력 생성
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                Debug.Log(err);
            }
            else
            {

                Debug.Log(contractAddress);
            }
        });



    }
   
    /*
    // NFT 토큰 데이터 클래스 정의
    public class NftTokenData
    {
        public BigInteger TokenId { get; set; }
        public string TokenURI { get; set; }

        public NftTokenData() { }
    }

    // 스마트 컨트랙트의 getNftTokenList 메서드 호출을 처리하는 함수
    public IEnumerator GetNftTokenListCoroutine(string ownerAddress)
    {
        Debug.Log(ownerAddress);
        var function = _contract.GetFunction("getNftTokenList");
        //var callInput = function.CreateCallInput(ownerAddress);

        var task = function.CallAsync<NftTokenData[]>(ownerAddress);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            NftTokenData[] tokenList = task.Result;
            foreach (var token in tokenList)
            {
                Debug.Log($"Token ID: {token.TokenId}, Token URI: {token.TokenURI}");
            }
        }
    }
    */

    private void Update()
    {
        StartCoroutine(MintNFT());
        //StartCoroutine(GetNftTokenListCoroutine(userAddress));
    }
}
