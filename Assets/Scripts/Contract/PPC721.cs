using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;


public class PPC721 : MonoBehaviour
{
    
    public PPC721Contract PPC721Contract;
    public PPCTokenContract TokenContract;
    public BigInteger tokenid = new System.Numerics.BigInteger(1);
    // Start is called before the first frame update
    private void Awake()
    {
        
        PPC721Contract.Initialize();
        
        //TokenContract.Initialize();
    }
    void OnEnable()
    {
        PPC721Contract.Initialize();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(GetNftPrice());
        }
    }

    //@notion ����ŷ��� UI�Ѹ� ����Ǵ� �Լ�
    //1.�Ǹŷ� �ö�� ���� ����Ʈ ����
    
    public IEnumerator GetNftPrice()
    {
        yield return StartCoroutine(PPC721Contract.GetNftPrice(new System.Numerics.BigInteger(1), (price, ex) => {
            if (ex == null)
            {
                BigInteger Tokenprice = price;
                Debug.Log($"Tokenprice for Token : {Tokenprice}");
            }
            else
            {
                Debug.Log(ex);
            }
        }));
    }
    
    
    
}
