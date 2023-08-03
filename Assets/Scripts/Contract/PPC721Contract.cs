using UnityEngine;
using System.Collections;
using System.Numerics;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using System.IO;
using System;


public class PPC721Contract : MonoBehaviour
{
    private Nethereum.Contracts.Contract _contract;
    private Web3 _web3;
    public static string userAddress;

    void Awake()
    {
        userAddress = "0x176feB0F409cecFd3362CD4C10fF730814368EfE";
        Initialize();
    }

    public void Initialize()
    {
        _web3 = new Web3("http://127.0.0.1:7545");
        var contractAbiPath = "Assets/contracts/PPC721 Contract.json";
        var contractAbi = ReadAbiFromFile(contractAbiPath);
        Debug.Log(contractAbi);
        _contract = _web3.Eth.GetContract(contractAbi, "0xAfdC75da5E1a8eAdb9B5B5c691A4Ed3fd04Fb829");
    }

    private string ReadAbiFromFile(string filePath)
    {
        try
        {
            string jsonContent = File.ReadAllText(filePath);
            var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
            var abiObject = jsonObject["abi"];
            return abiObject.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading ABI from file: {ex.Message}");
            return null;
        }

    }



    public IEnumerator MintNFTCoroutine()
    {
        string recipient = "0x176feB0F409cecFd3362CD4C10fF730814368EfE";
        string tokenURI = "https://gateway.pinata.cloud/ipfs/QmP25FavCnPjQpuvM9noxxfZNNRKW6cmUYsEg3LwSJ22gm/1.json";
        System.Numerics.BigInteger nftPrice = new System.Numerics.BigInteger(123);

        var function = _contract.GetFunction("mintNFT");
        var gas = new HexBigInteger(3000000);

        // ����Ʈ ��Ʈ��Ʈ�� �޼��忡 ������ Ʈ����� �Է� ����
        var transactionInput = function.CreateTransactionInput(PPC721Contract.userAddress, gas, gas, new object[] { recipient, tokenURI, nftPrice });
        Debug.Log(transactionInput.From);


        // SendTransactionAndWaitForReceiptAsync �޼���� Ʈ����� ���� �� ������ ���
        var task = function.SendTransactionAsync(transactionInput);
        yield return new WaitUntil(() => task.IsCompleted);
        // �񵿱� �۾� �Ϸ� ���θ� Ȯ���ϸ� ���
        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            Debug.Log("Transfer completed with transaction hash: " + task.Result);
        }
    }

    // NFT ��ū ������ Ŭ���� ����
    public class NftTokenData
    {
        public BigInteger TokenId { get; set; }
        public string TokenURI { get; set; }

        public NftTokenData() { }
    }

    // ����Ʈ ��Ʈ��Ʈ�� getNftTokenList �޼��� ȣ���� ó���ϴ� �Լ�
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

    private void Start()
    {
        StartCoroutine(MintNFTCoroutine());
        //StartCoroutine(GetNftTokenListCoroutine(userAddress));
    }
}
