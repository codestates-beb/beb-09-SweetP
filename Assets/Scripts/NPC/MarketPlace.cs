using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
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

    public void OnEnable()
    {
        AcquireSlot();
        print("able");
    }

    public void OnDisable()
    {
        MarketManager.instance.RefreshMarket();
        MarketManager.instance.GetMarket();
        marketSlotList.Clear();
        int childCount = MarketParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = MarketParent.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }


        print("disable");

    }

    public void GetWeaponDataById(int id, System.Action<WeaponData> callback)
    {
        string url = "https://breadmore.azurewebsites.net/api/Weapon_Data/weapon/" + id;
        HTTPClient.instance.GET(url, (www) =>
        {
            if (!string.IsNullOrEmpty(www))
            {
                WeaponData weaponData = JsonUtility.FromJson<WeaponData>(www);
                print(www);
                callback?.Invoke(weaponData);
            }
            else
            {
                callback?.Invoke(null);
            }
        });
    }

    public void AcquireSlot()
    {
        List<WeaponData> weaponDataList = new List<WeaponData>();
        List<Task> tasks = new List<Task>();

        // 비동기 작업들을 나타내는 Task 리스트 생성
        foreach (var marketData in MarketManager.instance.marketDataList)
        {
            int weaponId = marketData.weapon_id;
            Task task = GetWeaponDataAsync(weaponId, (weaponData) =>
            {
                weaponDataList.Add(weaponData);
            });
            tasks.Add(task);
            print(weaponId);
        }

        // 모든 Task가 완료될 때까지 대기한 후 슬롯 생성과 데이터 설정
        StartCoroutine(WaitForTasks(tasks, () =>
        {
            // 모든 작업이 완료되면 슬롯 생성과 데이터 설정을 수행합니다.
            for (int i = 0; i < weaponDataList.Count; i++)
            {
                GameObject instantiatedSlotGO = Instantiate(marketSlotPrefab, MarketParent.transform);
                MarketSlot marketSlot = instantiatedSlotGO.GetComponent<MarketSlot>();
                marketSlot.InitSlot(weaponDataList[i], MarketManager.instance.marketDataList[i]);

                marketSlotList.Add(marketSlot);
            }
        }));
    }

    private IEnumerator WaitForTasks(List<Task> tasks, System.Action onComplete)
    {
        yield return new WaitUntil(() => tasks.TrueForAll(t => t.IsCompleted));
        onComplete?.Invoke();
    }

    private async Task GetWeaponDataAsync(int id, System.Action<WeaponData> callback)
    {
        var tcs = new TaskCompletionSource<WeaponData>();
        GetWeaponDataById(id, (weaponData) =>
        {
            tcs.SetResult(weaponData);
        });
        WeaponData result = await tcs.Task;
        callback?.Invoke(result);
    }
}
