using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplementNFTStorage : MonoBehaviour
{
    string fullPath;
    public NFTStorage.NFTStorageClient NSC;

    public string body;
    private MetaDataWeapon metaDataWeapon;


    private void Start()
    {
        metaDataWeapon = new MetaDataWeapon();
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            print("hello");
            HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Weapon_Data/2", delegate (string www) {
                body =www;
                metaDataWeapon.weaponData = JsonUtility.FromJson<WeaponData>(www);
                metaDataWeapon.image = "https://naver.com";
                metaDataWeapon.name = "SweetP Weapon";
                print(www);
            });
        }




        //WeaponManager.instance.GetJSONWeaponData(1);

        if (Input.GetKeyDown(KeyCode.P))
        {
            string jsonData = JsonUtility.ToJson(metaDataWeapon);
            await NSC.UploadDataFromJsonHttpClient(jsonData);
        }
    }
}
