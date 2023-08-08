using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplementNFTStorage : MonoBehaviour
{
    string fullPath;
    public NFTStorage.NFTStorageClient NSC;

    private void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            fullPath = Application.persistentDataPath + "/myImage " + System.DateTime.Now.ToString("yy-MM-dd") + ".png";
            print(fullPath);
            ScreenCapture.CaptureScreenshot(fullPath);
        }




        //WeaponManager.instance.GetJSONWeaponData(1);

        if (Input.GetKey(KeyCode.P))
        {
            NSC.UploadDataFromStringUnityWebrequest(fullPath);
        }
    }
}
