using System.Collections;
using UnityEngine;
using System;
using System.IO;
using Nethereum.Web3;
using Nethereum.HdWallet;
using Nethereum.Web3.Accounts;

public class SmartContractInteraction : MonoBehaviour
{
    public Web3 web3;
    string jsonFileName;
    string abi;
    public string contractAddress;
    public static Account userAccount;
    public Nethereum.Contracts.Contract contract;
    //public string seedPhrase = "enough foil lawsuit tired replace pact awesome win autumn leisure armed pattern";
    public string seedPhrase = "inquiry vintage consider tell goat skill coffee across material seminar fence chronic";


    // Init함수는 jsonfile 이름으로 해당 컨트렉트 abi 가져온 후 abi와 contract 주소를 바탕으로 contract 인스턴스화 하는 과정이다.

    public void Init(string jsonFileName)
    {
        this.jsonFileName = jsonFileName;
        StartCoroutine(InstantiateContract());
    }

    public IEnumerator InstantiateContract()
    {
        yield return GetAbiFromJsonFile();
        if (string.IsNullOrEmpty(this.abi))
        {
            Debug.Log("None of abi exists");
            yield break;
        }
        if (string.IsNullOrEmpty(this.contractAddress))
        {
            Debug.Log("None of contract address exists");
            yield break;
        }

        try
        {
            // string url = "https://sepolia.infura.io/v3/7bcccb589f144d16a1b7871c29fdc6a4";
            string url = "http://127.0.0.1:8545";
            string privateKey = "5b4d0a80997d8816329efef5b803bffb9bdc758577a3ae10fef7839f53f1c987";
            string privateKey2 = "c1ee916e10ea495eba17f52d13549b52f8059a33f78c3ee43ceadec3a697cf07";
            string privateKey3 = "41980a25576d8f1144b41ca42f90c5c64f8fad016036f4b5fa74acd561a6678b";
            //var wallet = new Wallet(seedPhrase, "");
            //var privateKey = wallet.GetAccount(0).PrivateKey;

            userAccount = new Account(privateKey3);
            this.web3 = new Web3(userAccount, url);
            // 'this.web3' 필드에 'Web3' 인스턴스를 할당
            // ABI와 스마트 컨트랙트 주소로 스마트 컨트랙트 인스턴스 생성
            this.contract = this.web3.Eth.GetContract(this.abi, this.contractAddress);
        }
        catch (Exception ex)
        {
            Debug.Log($"Error : {ex.Message}");
        }
        yield return null;
    }

    private IEnumerator GetAbiFromJsonFile()
    {
        string path = "Assets/contracts/" + this.jsonFileName;
        try
        {
            // string path = Path.Combine(Application.dataPath, "contracts", jsonFileName);
            if (!File.Exists(path))
            {
                Debug.Log("File does not exist at: " + path);
                yield break;
            }
            string jsonContent = File.ReadAllText(path);
            var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
            var abiObject = jsonObject["abi"];
            var address = jsonObject["deployedAddress"];
            this.abi = abiObject.ToString();

            this.contractAddress = address.ToString();
        }
        catch (Exception ex)
        {
            Debug.Log($"Error: {ex.Message}");
        }
        yield return null;
    }
}