using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Text;
using NFTStorage.JSONSerialization;
using UnityEngine.Networking;

namespace NFTStorage.JSONSerialization
{
    [Serializable]
    public class NFTStorageError
    {
        public string name;
        public string message;
    }

    [Serializable]
    public class NFTStorageFiles
    {
        public string name;
        public string type;
    }

    [Serializable]
    public class NFTStorageDeal
    {
        public string batchRootCid;
        public string lastChange;
        public string miner;
        public string network;
        public string pieceCid;
        public string status;
        public string statusText;
        public int chainDealID;
        public string dealActivation;
        public string dealExpiration;
    }

    [Serializable]
    public class NFTStoragePin
    {
        public string cid;
        public string name;
        public string status;
        public string created;
        public int size;
        // TODO: add metadata parsing ('meta' property)
    }

    [Serializable]
    public class NFTStorageNFTObject
    {
        public string cid;
        public int size;
        public string created;
        public string type;
        public string scope;
        public NFTStoragePin pin;
        public NFTStorageFiles[] files;
        public NFTStorageDeal[] deals;
    }


    [Serializable]
    public class NFTStorageCheckValue
    {
        public string cid;
        public NFTStoragePin pin;
        public NFTStorageDeal[] deals;
    }

    [Serializable]
    public class NFTStorageGetFileResponse
    {
        public bool ok;
        public NFTStorageNFTObject value;
        public NFTStorageError error;
    }

    [Serializable]
    public class NFTStorageCheckResponse
    {
        public bool ok;
        public NFTStorageCheckValue value;
        public NFTStorageError error;
    }

    [Serializable]
    public class NFTStorageListFilesResponse
    {
        public bool ok;
        public NFTStorageNFTObject[] value;
        public NFTStorageError error;
    }

    [Serializable]
    public class NFTStorageUploadResponse
    {
        public bool ok;
        public NFTStorageNFTObject value;
        public NFTStorageError error;
    }

    [Serializable]
    public class NFTStorageDeleteResponse
    {
        public bool ok;
    }
}

namespace NFTStorage
{
    // This is the main class for communicating with nft.storage and IPFS
    public class NFTStorageClient : MonoBehaviour
    {
        // nft.storage endpoint
        private static readonly string nftStorageApiUrl = "https://api.nft.storage/";

        // HTTP client to communicate with nft.storage
        private static readonly HttpClient nftClient = new HttpClient();

        // nft.storage API key
        public string apiToken;

        // Start is called before the first frame update for initializing NFTStorageClient
        void Start()
        {
            nftClient.DefaultRequestHeaders.Add("Accept", "application/json");
            if (apiToken != null)
            {
                nftClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiToken);
            }
            else
            {
                Debug.Log("Starting NFT Storage Client without API key, please call 'SetApiToken' method before using class methods.");
            }
        }

        // Set API token to be used as authorization header for "nft.storage" HTTP API requests
        public void SetApiToken(string token)
        {
            apiToken = token;
            if (nftClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                nftClient.DefaultRequestHeaders.Remove("Authorization");
            }
            nftClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiToken);
        }

        // Upload JSON data using "nft.storage" HTTP API
        public async Task<NFTStorageUploadResponse> UploadDataFromJsonHttpClient(string jsonData)
        {
            string requestUri = nftStorageApiUrl + "/upload";
            var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await nftClient.PostAsync(requestUri, requestContent);

            if (response.IsSuccessStatusCode)
            {
                string rawResponse = await response.Content.ReadAsStringAsync();
                NFTStorageUploadResponse parsedResponse = JsonUtility.FromJson<NFTStorageUploadResponse>(rawResponse);
                return parsedResponse;
            }
            else
            {
                Debug.Log("Error while uploading JSON data: " + response.StatusCode);
                return null;
            }
        }
    }
}