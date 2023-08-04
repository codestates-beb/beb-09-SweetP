using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketPlace : MonoBehaviour
{
    private static MarketPlace _instance;
    public static MarketPlace instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = FindObjectOfType<MarketPlace>();
            }
            return _instance;
        }

    }

    [SerializeField]
    private GameObject MarketParent;

    public GameObject marketSlotPrefab;
    public List<MarketSlot> marketSlotList = new List<MarketSlot>();
    // Start is called before the first frame update
    void Start()
    {
        //AcquireSlot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetWeaponDataById(int id, System.Action<WeaponData> callback)
    {
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Weapon_Data/weapon/" + id, delegate (string www)
        {
            WeaponData weaponData = JsonUtility.FromJson<WeaponData>(www);
            callback?.Invoke(weaponData); // 콜백 함수 호출하여 데이터 전달
        });
    }

    public void AcquireSlot()
    {
        int childCount = MarketParent.transform.childCount;
        for(int i = childCount -1; i>=0; i--)
        {
            Transform child = MarketParent.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        for (int i = 0; i < MarketManager.instance.marketDataList.Count; i++)
        {
            int currentIndex = i;
            int weaponId = MarketManager.instance.marketDataList[i].weapon_id;
            GetWeaponDataById(weaponId, (weaponData) =>
            {
                // 콜백 함수 내에서 슬롯 생성 및 데이터 설정
                GameObject instantiatedSlotGO = Instantiate(marketSlotPrefab, MarketParent.transform);
                MarketSlot marketSlot = instantiatedSlotGO.GetComponent<MarketSlot>();
                marketSlot.InitSlot(weaponData, MarketManager.instance.marketDataList[currentIndex]);

                
                marketSlotList.Add(marketSlot);
            });
        }
    }
}
