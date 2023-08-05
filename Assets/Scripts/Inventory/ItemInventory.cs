using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    [SerializeField]
    private GameObject slotsParent;

    public ItemSlot[] Islots;

    public void AcquireScroll(ScrollData scrollData, int _count = 1)
    {
        for (int i = 0; i < Islots.Length; i++)
        {
            if (Islots[i].scrollData.count != 0)  // null 이라면 slots[i].item.itemName 할 때 런타임 에러 나서
            {
                if (Islots[i].scrollData.scrollType == scrollData.scrollType)
                {
                    Islots[i].SetSlotCount(_count);
                    return;
                }
            }
        }

        for (int i = 0; i < Islots.Length; i++)
        {
            if (Islots[i].scrollData.count == 0)
            {
                Islots[i].AddItem(scrollData, _count);
                return;
            }
        }

    }

    public void ClearItem()
    {
        for (int i = 0; i < Islots.Length; i++)
        {
            Islots[i].ClearSlot();
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
