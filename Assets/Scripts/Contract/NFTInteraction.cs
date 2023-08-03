/*
using UnityEngine;
using System.Numerics;
using System.Threading.Tasks;



public class NFTInteraction : MonoBehaviour
{
    

    async void Start()
    {
        var ppc721Contract = new PPC721Contract();
        string senderAddress = "0x176feB0F409cecFd3362CD4C10fF730814368EfE";
        string recipientAddress = "0x176feB0F409cecFd3362CD4C10fF730814368EfE";
        string tokenURI = "https://gateway.pinata.cloud/ipfs/QmP25FavCnPjQpuvM9noxxfZNNRKW6cmUYsEg3LwSJ22gm/1.json";
        BigInteger nftPrice = new BigInteger(10);

        string transactionHash = await ppc721Contract.MintNFTCoroutine(recipientAddress, tokenURI, nftPrice);
        Debug.Log($"Mint Transaction Hash: {transactionHash}");
    }
}
*/
