using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Nethereum.Hex.HexTypes;

public class PPC721 : MonoBehaviour
{

    public PPC721Contract PPC721Contract;
    public PPCTokenContract PPCTokenContract;
    public BigInteger tokenid = new System.Numerics.BigInteger(1);

    // Start is called before the first frame update

    private void Awake()
    {
        PPCTokenContract = GetComponent<PPCTokenContract>();
        //PPCTokenContract.Initialize();

        PPC721Contract = GetComponent<PPC721Contract>();



    }
    /*
    private void Awake()
    {
        PPC721Contract = new PPC721Contract();
        PPCTokenContract = new PPCTokenContract();
        //PPCTokenContract.Initialize();
    }
    */

    private void Start()
    {
        PPCTokenContract.Initialize();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //StartCoroutine(GetNftPrice());
            StartCoroutine(SetToken());
            //StartCoroutine(GetSaleWeapons());
            //StartCoroutine(BuyWeapon(new System.Numerics.BigInteger(2)));
        }
    }

    //@notion ����ŷ��� UI�Ѹ� ����Ǵ� �Լ�
    //1.�Ǹŷ� �ö�� ���� ����Ʈ ����

    public IEnumerator GetSaleWeapons()
    {
        StartCoroutine(SetToken());
        yield return StartCoroutine(PPC721Contract.GetSaleNftTokenIds((Weapons, ex) => {
            if (ex == null)
            {
                int[] WeaponsList = Weapons;
                for (int i = 0; i < WeaponsList.Length; i++)
                {
                    StartCoroutine(GetWeaponTokenURI(WeaponsList[i]));
                }
            }
            else
            {
                Debug.Log(ex);
            }
        }));
    }

    private IEnumerator GetWeaponTokenURI(int tokenId )
    {
        yield return StartCoroutine(PPC721Contract.GetTokenURI(tokenId, (url, ex) =>
        {
            string WeaponURl = url;
            Debug.Log($"Token {tokenId} URL: {WeaponURl}");
        }));
    }

    //@notion ����ŷ�
    //���� �ŷ� �� ������ ���� buy��ư�� ������ ����Ǵ� �Լ�
    
    public IEnumerator BuyWeapon(BigInteger tokenId)
    {
        //@notion ���⸦��ū���� ������� 721��Ʈ��Ʈ�� ��ū������ �ִ¾� ����
        yield return StartCoroutine(PPCTokenContract.Approve("0xb666d55294EfA8A8CCaCFdf1485e5D7484B92684", 20, (Address, ex) =>
        {
            Debug.Log($"Contract Address: {Address}");
        }));
        yield return StartCoroutine(PPC721Contract.BuyNftToken(tokenId, (Address, ex) =>
        {
            Debug.Log($"Contract Address: {Address}");
        }));

    }


    //@notion ��ū���� �ŷ��ϱ����� ERC20 ��ū�� �����ϴ� �Լ�
    public IEnumerator SetToken()
    {
        yield return StartCoroutine(PPC721Contract.SetToken("0xA3e30c7207b8E9Dfe1B405311cc4C0eDe2C02ac0", (Address, ex) =>
        {
            Debug.Log($"Contract Address: {Address}");
            StartCoroutine(GetToken());
        }));
    }
    //@notion ������ ERC20 ��ū�� Ȯ���ϴ� �Լ�
    public IEnumerator GetToken()
    {
        yield return StartCoroutine(PPC721Contract.GetToken((Address, ex) => {
            if (ex == null)
            {
                string TokenAddress = Address;
                Debug.Log($"TokenAddress : {TokenAddress}");
            }
            else
            {
                Debug.Log(ex);
            }
        }));
    }
    
    /*
    public IEnumerator Transfer()
    {
        yield return StartCoroutine(PPCTokenContract.Transfer("0xE503081665f268c99ff22F45Df5FC8f3A21Ef0C8", 1000, (Address, ex) =>
        {
            Debug.Log($"Contract Address: {Address}");
        }));
    }
    */

    //@notion ���� �Ǹ� ���
    //���� �Ǹ� ��ư�� ������ ����Ǵ� �Լ�
    public IEnumerator SaleWeapon(BigInteger tokenId, BigInteger price )
    {
        yield return StartCoroutine(PPC721Contract.SaleNftToken(tokenId, price, (Address, ex) =>
        {
            Debug.Log($"Contract Address: {Address}");
            Debug.Log($"SaleWeapon {tokenId} / price : {price}");
        }));
    }

    public IEnumerator GetIsSaleWeapon(BigInteger tokenId)
    {
        yield return StartCoroutine(PPC721Contract.GetIsSale(tokenId, (isSale, ex) => {
            if (ex == null)
            {
                bool isTokenSale = isSale;
                Debug.Log($"{tokenId} Token Sale : {isTokenSale}");
            }
            else
            {
                Debug.Log(ex);
            }
        }));
    }
}

