using System.Collections;
using UnityEngine;
using System;
using System.IO;
using Nethereum.Contracts;
using Nethereum.Web3;
using Newtonsoft.Json;

public class SmartContractInteraction : MonoBehaviour
{
    public Web3 web3;
    public string contractAddress;
    string jsonFileName;
    string abi;
    public string address;
    public Nethereum.Contracts.Contract contract;
    

    // Init함수는 jsonfile 이름으로 해당 컨트렉트 abi 가져온 후 abi와 contract 주소를 바탕으로 contract 인스턴스화 하는 과정이다.

    public void Init(string jsonFileName)
    {
        this.jsonFileName = jsonFileName;
        StartCoroutine(InstantiateContract());
    }
    
    public IEnumerator InstantiateContract()
    {
        yield return GetAbiFromJsonFile();
        if(string.IsNullOrEmpty(this.abi)) 
        {
            Debug.Log("None of abi exists");
            yield break;
        }
        if(string.IsNullOrEmpty(this.address)) {
            Debug.Log("None of contract address exists");
            yield break;
        }
        
        try {
            string url = "https://sepolia.infura.io/v3/7bcccb589f144d16a1b7871c29fdc6a4"; // Ethereum 노드 또는 Infura 엔드포인트 URL
            this.web3 = new Web3(url); // 'this.web3' 필드에 'Web3' 인스턴스를 할당
            // ABI와 스마트 컨트랙트 주소로 스마트 컨트랙트 인스턴스 생성
            this.contract = this.web3.Eth.GetContract(this.abi, this.address);
        }
        catch (Exception ex) {
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
            this.address = address.ToString();
        }
        catch(Exception ex) 
        {
            Debug.Log($"Error: {ex.Message}");
        }
        yield return null;
    }
}