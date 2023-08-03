using UnityEngine;
using Nethereum.Web3;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections;
using Nethereum.ABI.FunctionEncoding.Attributes;



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

    private void Update() {
        if(Input.GetKeyDown(KeyCode.N)) {
            //StartCoroutine(MintNFT());
            StartCoroutine(GetIsSale(new System.Numerics.BigInteger(1)));
        }
    }
    
    public IEnumerator MintNFT()
    {


        var function = this.contractInstance.contract.GetFunction("mintNFT");

        // Transaction Input 생성
        string recipient = "0x30018fC76ca452C1522DD9C771017022df8b2321";
        string tokenURI = "https://gateway.pinata.cloud/ipfs/QmP25FavCnPjQpuvM9noxxfZNNRKW6cmUYsEg3LwSJ22gm/1.json";
        System.Numerics.BigInteger nftPrice = new System.Numerics.BigInteger(1);
        var data = function.GetData(recipient, tokenURI, nftPrice);
        decimal ethValue = 0;

        // 스마트 컨트랙트의 메서드에 전달할 트랜잭션 입력 생성
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                Debug.Log(err);
            }
            else
            {

                Debug.Log("contractAddress" + contractAddress);
            }
        });



    }

    public IEnumerator UpdataNFT(BigInteger tokenId, string tokenURI)
    {
        var function = this.contractInstance.contract.GetFunction("updataNFT");

        var data = function.GetData(tokenId, tokenURI);
        decimal ethValue = 0;
        // 스마트 컨트랙트의 메서드에 전달할 트랜잭션 입력 생성
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                Debug.Log(err);
            }
            else
            {

                Debug.Log("contractAddress" + contractAddress);
            }
        });
    }

    public IEnumerator SetToken(string tokenAddress)
    {
        var function = this.contractInstance.contract.GetFunction("setToken");

        var data = function.GetData(tokenAddress);
        decimal ethValue = 0;
        // 스마트 컨트랙트의 메서드에 전달할 트랜잭션 입력 생성
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                Debug.Log(err);
            }
            else
            {

                Debug.Log("contractAddress" + contractAddress);
            }
        });

    }

    public IEnumerator SetNftPrice(BigInteger tokenId, BigInteger price)
    {
        var function = this.contractInstance.contract.GetFunction("setNftPrice");

        var data = function.GetData(tokenId, price);
        decimal ethValue = 0;

        // 스마트 컨트랙트의 메서드에 전달할 트랜잭션 입력 생성
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                Debug.Log(err);
            }
            else
            {

                Debug.Log("contractAddress" + contractAddress);
            }
        });
    }

    public IEnumerator GetNftPrice(BigInteger tokenId)
    {
        var function = this.contractInstance.contract.GetFunction("getNftPrice");
        var task = function.CallAsync<BigInteger>(tokenId);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            BigInteger Price = task.Result;
            Debug.Log($"NftPrice for Token {tokenId}: {Price}");
        }
    }

    public IEnumerator GetOwnerOfTokenId(BigInteger tokenId)
    {
        var function = this.contractInstance.contract.GetFunction("getOwnerOfTokenId");
        var task = function.CallAsync<string>(tokenId);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            string Address = task.Result;
            Debug.Log($"Owner Address for Token {tokenId}: {Address}");
        }

    }

    public IEnumerator SaleNftToken(BigInteger tokenId, BigInteger price)
    {
        var function = this.contractInstance.contract.GetFunction("saleNftToken");

        var data = function.GetData(tokenId, price);
        decimal ethValue = 0;

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                Debug.Log(err);
            }
            else
            {

                Debug.Log("contractAddress" + contractAddress);
            }
        });
    }

    public IEnumerator GetIsSale(BigInteger tokenId)
    {
        var function = this.contractInstance.contract.GetFunction("getIsSale");
        var task = function.CallAsync<bool>(tokenId);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            bool IsSale = task.Result;
            Debug.Log($"IsSale for Token {tokenId}: {IsSale}");
        }
    }


    public IEnumerator Approve(string approved, BigInteger tokenId)
    {
        var function = this.contractInstance.contract.GetFunction("approve");
        //string approved = "0xE503081665f268c99ff22F45Df5FC8f3A21Ef0C8";
        //System.Numerics.BigInteger tokenId = new System.Numerics.BigInteger(1);
        var data = function.GetData(approved, tokenId);
        decimal ethValue = 0;
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                Debug.Log(err);
            }
            else
            {

                Debug.Log("contractAddress" + contractAddress);
            }
        });

    }

    public IEnumerator GetApproved(BigInteger tokenId)
    {
        var function = this.contractInstance.contract.GetFunction("getApproved");
        var task = function.CallAsync<string>(tokenId);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            string approvedAddress = task.Result;
            Debug.Log($"Approved Address for Token {tokenId}: {approvedAddress}");
        }
    }

    public IEnumerator BuyNftToken(BigInteger tokenId)
    {
        var function = this.contractInstance.contract.GetFunction("buyNftToken");

        var data = function.GetData(tokenId);
        decimal ethValue = 0;
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
            if (contractAddress == null)
            {
                Debug.Log(err);
            }
            else
            {

                Debug.Log("contractAddress" + contractAddress);
            }
        });
    }







    // NFT 토큰 데이터 클래스 정의
    public class NftTokenData
    {
        public NftTokenData() // 기본 생성자 추가
        {
        }

        [Parameter("uint256", "nftTokenId", 1, false)]
        public BigInteger nftTokenId { get; set; }

        [Parameter("string", "nftTokenURI", 2, false)]
        public string nftTokenURI { get; set; }
    }



    // 스마트 컨트랙트의 getNftTokenList 메서드 호출을 처리하는 함수
    public IEnumerator GetNftTokenList(string ownerAddress)
    {
        Debug.Log(ownerAddress);
        var function = this.contractInstance.contract.GetFunction("getNftTokenList");

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
                Debug.Log($"Token ID: {token.nftTokenId}, Token URI: {token.nftTokenURI}");
            }
        }
    }


}
