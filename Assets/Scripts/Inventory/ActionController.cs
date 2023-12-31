using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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

    [SerializeField]
    private WeaponInventory marketWeaponPanel;
    private void CanPickUp()
    {

    }

    public async Task Refresh()
    {
        weaponInventory.ClearWeapon();

        weaponSelectPanel.ClearWeapon();

        marketWeaponPanel.ClearWeapon();

    }

    public async Task Init()
    {
        for (int i = 0; i < WeaponManager.instance.weaponDataList.Count; i++)
        {
            weaponInventory.AcquireWeapon(WeaponManager.instance.weaponDataList[i]);

            weaponSelectPanel.AcquireWeapon(WeaponManager.instance.weaponDataList[i]);

            marketWeaponPanel.AcquireWeapon(WeaponManager.instance.weaponDataList[i]);
        }

        
            itemInventory.AcquireScroll();

            itemSelectPanel.AcquireScroll();
        
    }

    private async void Start()
    {
        await RefreshAll();
    }

    public async Task RefreshAll()
    {
        await Refresh();
        await Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
