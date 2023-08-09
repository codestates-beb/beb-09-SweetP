using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager _instance;
    public static WeaponManager instance
    {

        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<WeaponManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    public List<Material> upgradeColor = new List<Material>();
    public List<GameObject> upgradeParticle = new List<GameObject>();

    public List<WeaponData> weaponDataList = new List<WeaponData>();
    private class WeaponTBArray
    {
        public WeaponTB[] weapons;
    }

    public WeaponData curruentWeaponData;
    private bool currentWeaponCheck = false;

    public GameObject Sword;
    public GameObject Bow;
    public GameObject Magic;

    // Start is called before the first frame update

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Refresh();
        GetWeaponList();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Refresh()
    {
        print("weapon Refresh");
        weaponDataList.Clear();
    }

    public void ChangeWeaponData(WeaponData weapondata)
    {
        for(int i=0; i<weaponDataList.Count; i++)
        {
            if(weaponDataList[i].weapon_id == weapondata.weapon_id)
            {
                weaponDataList[i] = weapondata;
            }
        }
    }
    private void HandleWeaponTB(string jsonData)
    {
        WeaponTBArray weaponTBArray = JsonUtility.FromJson<WeaponTBArray>("{\"weapons\":"+jsonData+"}");
        int count = 0;
        if(weaponTBArray != null && weaponTBArray.weapons != null)
        {
            foreach (var weaponTB in weaponTBArray.weapons)
            {
                int weapon_id = weaponTB.weapon_id;
                int weapon_owenr = weaponTB.weapon_owner;

                GetWeaponData(weapon_id);
                count++;
            }
            
            
        }
        else
        {
            Debug.LogError("JSON NULL");
        }
    }

    private void HandleWeaponData(string jsonData)
    {
        WeaponData weaponData = JsonUtility.FromJson<WeaponData>(jsonData);
        for(int i=0; i<weaponDataList.Count; i++)
        {
            if(weaponDataList[i].weapon_id == weaponData.weapon_id)
            {
                return;
            }
        }
        weaponDataList.Add(weaponData);
    }

    public void GetWeaponList()
    {
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Weapon_TB/owner/" + LoginManager.instance.PlayerID, delegate (string www)
        {
            HandleWeaponTB(www);
        });
    }


    public void GetWeaponData(int weapon_id)
    {
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Weapon_Data/weapon/" + weapon_id, delegate (string www)
        {
            HandleWeaponData(www);
        });
    }

    public void EquipWeapon(WeaponData weaponData)
    {
        if (PlayerAttack.instance.pivotWeaponR.transform.childCount > 0)
        {
            Destroy(PlayerAttack.instance.pivotWeaponR.GetChild(0).gameObject);
        }
        GameObject weapon = new GameObject();
        curruentWeaponData = weaponData;


        switch (curruentWeaponData.weapon_type)
        {

            case 0:
                weapon = Instantiate(Sword, PlayerAttack.instance.pivotWeaponR);
                break;

            //bow
            case 1:
                break;

            //magic
            case 2:
                weapon = Instantiate(Magic, PlayerAttack.instance.pivotWeaponR);
                break;
        }

        PlayerAttack.instance.objWeapon = weapon;
        PlayerAttack.instance.objWeapon.GetComponent<MeshRenderer>().material = upgradeColor[weaponData.weapon_upgrade / 2];
        PlayerAttack.instance.colliderWeapon = PlayerAttack.instance.objWeapon.GetComponent<BoxCollider>();
        UpgradeParticle(weaponData, PlayerAttack.instance.objWeapon);
        PlayerAttack.instance.colliderWeapon.enabled = false;
        PlayerAttack.instance.IsWeaponEquip = true;
        Player.instance.ChangeHealthWithWeapon();
    }

    public void WeaponUse(WeaponData currentWeapon)
    {
        currentWeapon.weapon_durability--;
        for(int i=0; i < weaponDataList.Count; i++)
        {
            if(weaponDataList[i].weapon_id == currentWeapon.weapon_id)
            {
                weaponDataList[i] = currentWeapon;
                if(DestroyWeapon(weaponDataList[i]))
                {
                    weaponDataList[i] = null;
                    return;
                }
            }
        }

        string url = "https://breadmore.azurewebsites.net/api/Weapon_Data/" + currentWeapon.weapon_id;
        string body = JsonUtility.ToJson(currentWeapon);

        HTTPClient.instance.PUT(url, body, (response) =>
        {

        });


    }

    public void UpgradeParticle(WeaponData weaponData ,GameObject objWeapon)
    {
        GameObject particleInstance;

        switch (weaponData.weapon_element)
        {
            case WeaponNature.Fire:
                particleInstance = Instantiate(upgradeParticle[0], objWeapon.transform);
                
                break;
            case WeaponNature.Water:
                particleInstance = Instantiate(upgradeParticle[0], objWeapon.transform);
                break;
            case WeaponNature.Thunder:
                particleInstance = Instantiate(upgradeParticle[0], objWeapon.transform);
                break;
            case WeaponNature.Earth:
                particleInstance = Instantiate(upgradeParticle[0], objWeapon.transform);
                break;
        }
    }
    public void createWeapon()
    {
        switch (curruentWeaponData.weapon_type)
        {
            //sword
            case 0:
                Instantiate(Sword);
                break;

            // bow
            case 1:
                break;

            // magic
            case 2:
                break;
        }
    }

    public bool DestroyWeapon(WeaponData weaponData)
    {
        if (weaponData.weapon_durability <= 0)
        {
            string url = "https://breadmore.azurewebsites.net/api/Weapon_Data/" + weaponData.weapon_id;
            HTTPClient.instance.DELETE(url, delegate (string www) { });

            return true;
        }
        return false;
    }

    public string GetJSONWeaponData(int weaponId)
    {
        for(int i=0; i<weaponDataList.Count; i++)
        {
            if(weaponDataList[i].weapon_id == weaponId)
            {

                return JsonUtility.ToJson(weaponDataList[i]);
            }
        }

        return null;
    }

}
