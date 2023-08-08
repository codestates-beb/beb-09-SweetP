using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplementNFTStorage : MonoBehaviour
{
    private static ImplementNFTStorage _instacne;
    public static ImplementNFTStorage instance
    {
        get
        {

            if(_instacne == null)
            {
                _instacne = FindObjectOfType<ImplementNFTStorage>();
            }
            return _instacne;
        }
    }
    public NFTStorage.NFTStorageClient NSC;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
