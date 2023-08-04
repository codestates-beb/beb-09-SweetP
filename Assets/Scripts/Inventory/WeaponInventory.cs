using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField]
    private GameObject slotsParent;

    public EquipSlot[] Eslots;

    // Start is called before the first frame update
    void Start()
    {
        //Eslots = slotsParent.GetComponentsInChildren<EquipSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcquireWeapon(WeaponData weaponData)
    {
        for (int i = 0; i < Eslots.Length; i++)
        {
            //try this
            if (Eslots[i].weaponData.weapon_id == weaponData.weapon_id)
            {
                return;
            }
            
        }

        for (int i=0; i< Eslots.Length; i++)
        {
            if(Eslots[i].weaponData.weapon_id == 0)
            {
                Eslots[i].AddWeapon(weaponData);
                return;
            }
        }
    }

    public void ClearWeapon()
    {
        for (int i = 0; i < Eslots.Length; i++)
        {
                Eslots[i].ClearSlot();
        }
    }
}
