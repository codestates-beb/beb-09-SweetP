using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    [SerializeField]
    private GameObject slotsParent;

    public ItemSlot[] Islots;

    public void AcquireScroll()
    {
        for (int i = 0; i < Islots.Length; i++)
        {
            Islots[i].SetItem();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
