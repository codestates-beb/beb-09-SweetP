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

    public void Initialize()
    {
        contractInstance.Init("PPC721Contract.json");
    }

    /*
    private IEnumerator UpdateCoroutine()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            yield return StartCoroutine(PPC721Contract.GetTokenURI(new System.Numerics.BigInteger(1), (TokenIds, ex) => {
                if (ex == null)
                {
                    string SaleTokenIds = TokenIds;
                    Debug.Log($"All Nft Ids for Token : {(SaleTokenIds)}");
                }
                else
                {
                    Debug.Log(ex);
                }
            }));
        }

        //yield return null;
    }
    */

    public IEnumerator MintNFT(Action<string, Exception> callback)
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
                callback(null, err);
            }
            else
            {
                callback(contractAddress, null);
            }
        });



    }

    public IEnumerator UpdataNFT(BigInteger tokenId, string tokenURI, BigInteger tokenAmount, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("updataNFT");

        var data = function.GetData(tokenId, tokenURI, tokenAmount);
        decimal ethValue = 0;
        // 스마트 컨트랙트의 메서드에 전달할 트랜잭션 입력 생성
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
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

    public IEnumerator BurnNFT(BigInteger tokenId, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("burnNFT");

        var data = function.GetData(tokenId);
        decimal ethValue = 0;
        // 스마트 컨트랙트의 메서드에 전달할 트랜잭션 입력 생성
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
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

    public IEnumerator GetTokenURI(BigInteger tokenId, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("getTokenURI");
        var task = function.CallAsync<string>(tokenId);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            try
            {
                string TokenURI = task.Result;
                callback(TokenURI, null);
            }
            catch (System.Exception ex)
            {
                callback(null, ex);
            }
        }
    }

    public IEnumerator SetToken(string tokenAddress, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("setToken");

        var data = function.GetData(tokenAddress);
        decimal ethValue = 0;
        // 스마트 컨트랙트의 메서드에 전달할 트랜잭션 입력 생성
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
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

    public IEnumerator GetToken(Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("getToken");
        var task = function.CallAsync<string>();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            try
            {
                string tokenAddress = task.Result;
                callback(tokenAddress, null);
            }
            catch (System.Exception ex)
            {
                callback(null, ex);
            }

        }
    }

    public IEnumerator SetNftPrice(BigInteger tokenId, BigInteger price, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("setNftPrice");

        var data = function.GetData(tokenId, price);
        decimal ethValue = 0;

        // 스마트 컨트랙트의 메서드에 전달할 트랜잭션 입력 생성
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
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

    public IEnumerator GetNftPrice(BigInteger tokenId, Action<BigInteger, Exception> callback)
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
            try
            {
                BigInteger Price = task.Result;
                callback(Price, null);
            }
            catch (System.Exception ex)
            {
                callback(new System.Numerics.BigInteger(-1), ex);
            }
            
        }
    }

    public IEnumerator GetOwnerOfTokenId(BigInteger tokenId, Action<string, Exception> callback)
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
            try
            {
                string Address = task.Result;
                callback(Address, null);
            }
            catch (System.Exception ex)
            {
                callback(null, ex);
            }
        }

    }

    public IEnumerator SaleNftToken(BigInteger tokenId, BigInteger price, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("saleNftToken");

        var data = function.GetData(tokenId, price);
        decimal ethValue = 0;

        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
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

    public IEnumerator GetIsSale(BigInteger tokenId, Action<bool, Exception> callback)
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
            try
            {
                bool IsSale = task.Result;
                callback(IsSale, null);
            }
            catch (System.Exception ex)
            {
                callback(false, ex);
            }
            //bool IsSale = task.Result;
            //Debug.Log($"IsSale for Token {tokenId}: {IsSale}");
        }
    }


    public IEnumerator Approve(string approved, BigInteger tokenId, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("approve");
        //string approved = "0xE503081665f268c99ff22F45Df5FC8f3A21Ef0C8";
        //System.Numerics.BigInteger tokenId = new System.Numerics.BigInteger(1);
        var data = function.GetData(approved, tokenId);
        decimal ethValue = 0;
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
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

    public IEnumerator GetApproved(BigInteger tokenId, Action<string, Exception> callback)
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
            try
            {
                string approvedAddress = task.Result;
                callback(approvedAddress, null);
            }
            catch (System.Exception ex)
            {
                callback(null, ex);
            }
        }
    }

    public IEnumerator BuyNftToken(BigInteger tokenId, Action<string, Exception> callback)
    {
        var function = this.contractInstance.contract.GetFunction("buyNftToken");

        var data = function.GetData(tokenId);
        decimal ethValue = 0;
        yield return FrequentlyUsed.SendTransaction(this.contractInstance, this.contractInstance.contractAddress, ethValue, data, (contractAddress, err) => {
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
    /*
    [System.Serializable]
    public class StringArrayWrapper
    {
        public int[] array;
    }
     */

    public IEnumerator GetAllNftIds(Action<int[], Exception> callback)
    {
        char separator = ','; // 구분자
        var function = this.contractInstance.contract.GetFunction("getAllNftIds");
        var task = function.CallAsync<string>();
        // 스마트 컨트랙트 함수 호출
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            string NftIds = task.Result;
            string[] NftIdsArray = NftIds.Split(separator);

            // 문자열 배열을 정수 배열로 변환
            int[] intNftIdsArray = new int[NftIdsArray.Length];
            for (int i = 0; i < NftIdsArray.Length; i++)
            {
                if (int.TryParse(NftIdsArray[i], out int parsedValue))
                {
                    intNftIdsArray[i] = parsedValue;
                    //StartCoroutine(GetTokenURI(parsedValue));
                    //Debug.Log($"parse value: {NftIdsArray[i]}");
                }
                else
                {

                    Debug.LogWarning($"Failed to parse value: {NftIdsArray[i]}");
                }
            }
            try
            {
                //ethBalance = Web3.Convert.FromWei(task.Result.Value);
                callback(intNftIdsArray, null);
            }
            catch (System.Exception ex)
            {
                callback(null, ex);
            }
            Debug.Log($"All Nft Ids for Token : {string.Join(", ", intNftIdsArray)}");
        }
    }

    public IEnumerator GetTokenIdOwner(string owner, Action<int[], Exception> callback)
    {
        char separator = ','; // 구분자
        var function = this.contractInstance.contract.GetFunction("getTokenIdOwner");
        var task = function.CallAsync<string>(owner);
        // 스마트 컨트랙트 함수 호출
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            string NftIds = task.Result;
            string[] NftIdsArray = NftIds.Split(separator);

            // 문자열 배열을 정수 배열로 변환
            int[] intNftIdsArray = new int[NftIdsArray.Length];
            for (int i = 0; i < NftIdsArray.Length; i++)
            {
                if (int.TryParse(NftIdsArray[i], out int parsedValue))
                {
                    intNftIdsArray[i] = parsedValue;
                    //Debug.Log($"parse value: {NftIdsArray[i]}");
                }
                else
                {
                    Debug.LogWarning($"Failed to parse value: {NftIdsArray[i]}");
                }
            }
            try
            {
                //ethBalance = Web3.Convert.FromWei(task.Result.Value);
                callback(intNftIdsArray, null);
            }
            catch (System.Exception ex)
            {
                callback(null, ex);
            }
            Debug.Log($"All Nft Ids for Token : {string.Join(", ", intNftIdsArray)}");
        }
    }


    public IEnumerator GetSaleNftTokenIds(Action<int[], Exception> callback)
    {
        char separator = ','; // 구분자
        var function = this.contractInstance.contract.GetFunction("getSaleNftTokenIds");
        var task = function.CallAsync<string>();
        // 스마트 컨트랙트 함수 호출
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            string NftIds = task.Result;
            string[] NftIdsArray = NftIds.Split(separator);

            // 문자열 배열을 정수 배열로 변환
            int[] intNftIdsArray = new int[NftIdsArray.Length];
            for (int i = 0; i < NftIdsArray.Length; i++)
            {
                if (int.TryParse(NftIdsArray[i], out int parsedValue))
                {
                    intNftIdsArray[i] = parsedValue;
                    //Debug.Log($"parse value: {NftIdsArray[i]}");
                }
                else
                {
                    Debug.LogWarning($"Failed to parse value: {NftIdsArray[i]}");
                }
            }
            try
            {
                //ethBalance = Web3.Convert.FromWei(task.Result.Value);
                callback(intNftIdsArray, null);
            }
            catch (System.Exception ex)
            {
                callback(null, ex);
            }
            Debug.Log($"All Nft Ids for Token : {string.Join(", ", intNftIdsArray)}");
        }
    }

    public IEnumerator GetSaleNftTokenIdsByAddress(string owner, Action<int[], Exception> callback)
    {
        char separator = ','; // 구분자
        var function = this.contractInstance.contract.GetFunction("getSaleNftTokenIdsByAddress");
        var task = function.CallAsync<string>(owner);
        // 스마트 컨트랙트 함수 호출
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            string NftIds = task.Result;
            string[] NftIdsArray = NftIds.Split(separator);

            // 문자열 배열을 정수 배열로 변환
            int[] intNftIdsArray = new int[NftIdsArray.Length];
            for (int i = 0; i < NftIdsArray.Length; i++)
            {
                if (int.TryParse(NftIdsArray[i], out int parsedValue))
                {
                    intNftIdsArray[i] = parsedValue;
                    //Debug.Log($"parse value: {NftIdsArray[i]}");
                }
                else
                {
                    Debug.LogWarning($"Failed to parse value: {NftIdsArray[i]}");
                }
            }
            try
            {
                //ethBalance = Web3.Convert.FromWei(task.Result.Value);
                callback(intNftIdsArray, null);
            }
            catch (System.Exception ex)
            {
                callback(null, ex);
            }
            Debug.Log($"All Nft Ids for Token : {string.Join(", ", intNftIdsArray)}");
        }
    }

}
