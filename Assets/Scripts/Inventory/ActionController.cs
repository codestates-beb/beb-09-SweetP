using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    private static ActionController _instance;
    public static ActionController instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<ActionController>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private WeaponInventory weaponInventory;
    [SerializeField]
    private ItemInventory itemInventory;

    [SerializeField]
    private WeaponInventory weaponSelectPanel;
    [SerializeField]
    private ItemInventory itemSelectPanel;
    private void CanPickUp()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i< WeaponManager.instance.weaponDataList.Count; i++)
        {
            weaponInventory.AcquireWeapon(WeaponManager.instance.weaponDataList[i]);

            weaponSelectPanel.AcquireWeapon(WeaponManager.instance.weaponDataList[i]);
        }

        for(int i=0; i < ItemManager.instance.scrollDataList.Count; i++)
        {
            itemInventory.AcquireScroll(ItemManager.instance.scrollDataList[i], ItemManager.instance.scrollDataList[i].count);

            itemSelectPanel.AcquireScroll(ItemManager.instance.scrollDataList[i], ItemManager.instance.scrollDataList[i].count);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
