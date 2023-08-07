using System.Collections;
using UnityEngine;
using System;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Numerics;

public class PPC20Contract : MonoBehaviour
{
    public string contractAddress = "0xAfdC75da5E1a8eAdb9B5B5c691A4Ed3fd04Fb829"; // ���ϴ� ����Ʈ ��Ʈ��Ʈ �ּҸ� �Է��ϼ���
    public Web3 web3;
    public decimal tokenBalance;
    private string abi; // ABI ������ ������ ����

    void Awake()
    {
        web3 = new Web3("http://127.0.0.1:7545"); // Ethereum ��� �Ǵ� Infura ��������Ʈ URL
        Initialize();
    }

    // ��Ʈ��Ʈ �ʱ�ȭ �� ABI ������ �о���� �Լ�
    public void Initialize()
    {
        string abiPath = "Assets/contracts/PPC20 Contract.json";

        abi = ReadAbiFromFile(abiPath); // ���Ͽ��� ABI ���� �о����

        Debug.Log(abi);

        
    }

    // ���Ͽ��� ABI ���� �о���� �Լ�
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

    public IEnumerator BalanceOf(string account, Action<decimal, Exception> callback)
    {
        if (string.IsNullOrEmpty(abi) || string.IsNullOrEmpty(contractAddress))
        {
            Debug.LogError("ABI or contract address is missing");
            yield break;
        }

        var contract = web3.Eth.GetContract(abi, contractAddress);
        var function = contract.GetFunction("balanceOf");
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

    public IEnumerator Transfer(string fromAddress, string recipient, decimal amount)
    {
        if (string.IsNullOrEmpty(abi) || string.IsNullOrEmpty(contractAddress))
        {
            Debug.LogError("ABI or contract address is missing");
            yield break;
        }

        var contract = web3.Eth.GetContract(abi, contractAddress);
        var function = contract.GetFunction("transfer");
        var weiAmount = BigInteger.Parse((amount * (decimal)Math.Pow(10, 18)).ToString("0"));
        var transactionInput = function.CreateTransactionInput(fromAddress, new object[] { recipient, weiAmount });
        transactionInput.Gas = new HexBigInteger(new BigInteger(3000000)); // ���÷� 3000000�� �����߽��ϴ�.

        var task = function.SendTransactionAsync(transactionInput);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            Debug.Log("Transfer completed with transaction hash: " + task.Result);
        }
    }

    private void OnBalanceReceived(decimal tokenBalance, Exception exception)
    {
        if (exception != null)
        {
            Debug.LogError(exception);
        }
        else
        {
            Debug.Log("Token balance: " + tokenBalance);
        }
    }

    void Start()
    {
        StartCoroutine(BalanceOf("0x176feB0F409cecFd3362CD4C10fF730814368EfE", OnBalanceReceived));
        StartCoroutine(Transfer("0x176feB0F409cecFd3362CD4C10fF730814368EfE", "0x6b002167e265668b505F78AC3792D4eF4E2C49C7", 10));
    }

}
