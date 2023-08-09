using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using NFTStorage.JSONSerialization;
using System.Threading.Tasks;


public class Enemy : LivingEntity
{
    

    public float currentTime;

    //@notion 컨트랙트 컴포넌트
    public PPC721Contract PPC721Contract;

    public string body;
    private MetaDataWeapon metaDataWeapon;
    
    //public enum Type { Normal, Boss};
    public GameObject healthBarPrefab;
    public Slider healthSlider;
    private GameObject healthBarInstance;

    public LayerMask whatIsTarget; // 추적 대상 레이어
    public BoxCollider AttackRange;
    public LivingEntity targetEntity;
    private NavMeshAgent pathFinder;

    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;
    public AudioClip dropSound;
    public AudioClip attackSound;
    private Animator enemyAnimator;
    private AudioSource enemyAudioPlayer;
    private Renderer enemyRenderer;

    public Transform playerTransform;

    public bool CantAction = false;
    private bool inDistance = false;
    private bool isChase = true;
    private bool isAttack = false;
    public float damage = 10f;
    public float timeBetAttack = 5.5f;
    public float attackDistance = 5f;
    private float lastAttackTime;

    [HideInInspector]
    public bool isBoss = false;
    private int score = 0;
    [HideInInspector]
    public float currentTimeForScore;


    [Header("Gold")]
    public int dropGold = 0;
    public int dropGoldMin = 10;
    public int dropGoldMax = 25;

    [Header("Drop Scroll")]
    public List<ScrollDropTable> scrollDropTables = new List<ScrollDropTable>();

    [Header("Drop Weapon")]
    public List<WeaponDropTable> weaponDropTables = new List<WeaponDropTable>();
    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool hasTarget
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }

    void Targeting()
    {


        float targetRadius = 1.5f;
        RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position,
                targetRadius,
                transform.forward,
                attackDistance,
                LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
        {
            inDistance = true;
            enemyAnimator.SetBool("InDistance", inDistance);
            if (!CantAction)
            {
                if (Time.time >= lastAttackTime + timeBetAttack)
                {
                    enemyAnimator.SetTrigger("Attack");
                    isChase = false;
                    pathFinder.isStopped = true;
                    isAttack = true;
                }
            }
        }
        if (hasTarget && rayHits.Length <= 0 && isChase == false)
        {
            isChase = true;
            pathFinder.isStopped = false;

        }
    }

    private void Awake()
    {
        // 초기화
        PPC721Contract =  GameObject.FindGameObjectWithTag("Contract").GetComponent<PPC721Contract>();

        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();

        enemyRenderer = GetComponentInChildren<Renderer>();
        lastAttackTime = 0f;

        healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        healthBarInstance.transform.SetParent(GameObject.Find("FieldCanvas").transform);

        healthSlider = healthBarInstance.GetComponent<Slider>();


    }

    // 적 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        startingHealth = newHealth;
        health = newHealth;
        damage = newDamage;
        pathFinder.speed = newSpeed;
        enemyRenderer.material.color = skinColor;

        //healthSlider
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
    }

    // Start is called before the first frame update
    void Start()
    {
        metaDataWeapon = new MetaDataWeapon();
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (!dead)
        {
            if (CantAction)
            {
                pathFinder.isStopped = true;
            }
            else
            {
                pathFinder.isStopped = false;
            }
            Vector3 healthBarPosition = transform.position + new Vector3(0f, 2f, 0f);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(healthBarPosition);
            healthBarInstance.transform.position = screenPosition;

            enemyAnimator.SetBool("HasTarget", hasTarget);
            Targeting();
        }

    }
    private IEnumerator UpdatePath()
    {
        // 살아있는 동안 무한 루프
        while (!dead)
        {
            if (hasTarget)
            {
                if (!isChase)
                {
                    pathFinder.isStopped = true;
                }
                else
                {
                    pathFinder.isStopped = false;
                    Vector3 targetPosition = targetEntity.transform.position;
                    pathFinder.SetDestination(targetEntity.transform.position);
                }
                if (isAttack)
                {
                    pathFinder.isStopped = true;
                }
            }
            else
            {
                pathFinder.isStopped = true;
                Collider[] colliders = Physics.OverlapSphere(
                    transform.position, 20f, whatIsTarget);

                for (int i = 0; i < colliders.Length; i++)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;

                        break;
                    }
                }
            }
            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

    // 데미지를 입었을때 실행할 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용

        if (!dead)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            enemyAudioPlayer.PlayOneShot(hitSound);
        }
        base.OnDamage(damage, hitPoint, hitNormal);

        healthSlider.value = health;
        print(health);
        print(healthSlider.value);
    }

    // 사망 처리
    public override async void Die()
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        if (isBoss)
        {
            RecordScore();
        }

        Collider enemyCollider = GetComponent<Collider>();

        enemyCollider.enabled = false;


        pathFinder.isStopped = true;
        pathFinder.enabled = false;

        dropGold = Random.Range(dropGoldMin, dropGoldMax);
        enemyAnimator.SetTrigger("Die");
        enemyAudioPlayer.PlayOneShot(deathSound);
        healthSlider.gameObject.SetActive(false);
        ItemManager.instance.ChangeGold(dropGold);
        
        WeaponManager.instance.WeaponUse(WeaponManager.instance.curruentWeaponData);

        InvisiableEnemy();

        await DropScoll();
        await DropWeapon();
        StartCoroutine(DestroyEnemy());
    }

    private void InvisiableEnemy()
    {
        Renderer renderer = GetComponent<Renderer>();
        
        renderer.material = UIManager.instance.material;
    }

    private IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(25f);
        Destroy(gameObject);
    }

    public void RecordScore()
    {
        score = (int)(Time.time - currentTimeForScore);

        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Player_Record/" + LoginManager.instance.PlayerID, delegate (string www)
        {
            PlayerRecord _playerRecord = JsonUtility.FromJson<PlayerRecord>(www);

            int maxScore = _playerRecord.player_score;

            if (maxScore > score)
            {
                PlayerRecord playerRecord = new PlayerRecord();
                playerRecord.player_id = LoginManager.instance.PlayerID;
                playerRecord.player_score = score;

                string body = JsonUtility.ToJson(playerRecord);
                print(body);
                HTTPClient.instance.PUT("https://breadmore.azurewebsites.net/api/Player_Record/" + LoginManager.instance.PlayerID, body, delegate (string www)
                {
                    print("Record");
                });
            }
        });
    }

    void Attack()
    {
        enemyAudioPlayer.PlayOneShot(attackSound);
        AttackRange.enabled = true;
    }

    void EndAttack()
    {
        isAttack = false;
        AttackRange.enabled = false;

        lastAttackTime = Time.time;
        //Debug.Log("Last Attack : "+lastAttackTime);
    }

    private async Task DropScoll()
    {
        for (int i = 0; i < scrollDropTables.Count; i++)
        {
            if (scrollDropTables[i].dropProb != 0)
            {
                if (Random.value < (scrollDropTables[i].dropProb / 100))
                {
                    enemyAudioPlayer.PlayOneShot(dropSound);
                    switch (scrollDropTables[i].scrollType)
                    {
                        case (ScrollType)0:
                            ItemManager.instance.scrollData.normal++;
                            break;
                        case (ScrollType)1:
                            ItemManager.instance.scrollData.unique++;
                            break;
                        case (ScrollType)2:
                            ItemManager.instance.scrollData.legendary++;
                            break;
                    }
                    ItemManager.instance.InitScroll();
                    ItemManager.instance.GetScroll();
                }
            }

        }
    }

    private async Task DropWeapon()
    {

        for (int i = 0; i < weaponDropTables.Count; i++)
        {
            WeaponTB weaponTB = new WeaponTB();
            if (weaponDropTables[i].dropProb != 0)
            {
                if (Random.value < (weaponDropTables[i].dropProb / 100))
                {
                    enemyAudioPlayer.PlayOneShot(dropSound);
                    switch (weaponDropTables[i].weapon_type)
                    {
                        case 0:
                            print("hello");
                            weaponTB.weapon_owner = LoginManager.instance.PlayerID;
                            string body = JsonUtility.ToJson(weaponTB);
                            HTTPClient.instance.POST("https://breadmore.azurewebsites.net/api/Weapon_tb", body, delegate (string www)
                             {
                                 HandleWeaponTB(www, (weaponData) =>
                                 {
                                     // 여기서 반환된 weaponData를 활용하여 원하는 작업 수행
                                     // 예: Debug.Log(weaponData.weapon_id);
                                     string jsonData = GetJsonWeaponData(weaponData);
                                     print(jsonData);
                                     setIPFS(jsonData);
                                     
                                 });
                             });
                            break;
                        case 1:
                            break;
                        case 2:
                            print("hello");
                            weaponTB.weapon_owner = LoginManager.instance.PlayerID;
                            string staffBody = JsonUtility.ToJson(weaponTB);
                            HTTPClient.instance.POST("https://breadmore.azurewebsites.net/api/Weapon_tb", staffBody, delegate (string www)
                            {
                                HandleWeaponTB(www, (weaponData) =>
                                {
                                    // 여기서 반환된 weaponData를 활용하여 원하는 작업 수행
                                    // 예: Debug.Log(weaponData.weapon_id);

                                    string jsonData = GetJsonWeaponData(weaponData);
                                    print(jsonData);
                                    setIPFS(jsonData);

                                });
                            });
                            break;
                    }
                }
            }



        }
    }

    private void HandleWeaponTB(string www, System.Action<WeaponData> callback)
    {
        WeaponTB weaponTb = JsonUtility.FromJson<WeaponTB>(www);

        // GET 요청 보내기
        HTTPClient.instance.GET("https://breadmore.azurewebsites.net/api/Weapon_Data/" + weaponTb.weapon_id, delegate (string response)
        {
            // 응답 데이터를 WeaponData 객체로 변환
            WeaponData weaponData = JsonUtility.FromJson<WeaponData>(response);

            // 콜백 호출하여 WeaponData 객체를 반환
            callback?.Invoke(weaponData);
        });
    }

    private string GetJsonWeaponData(WeaponData weaponData)
    {
        string jsonData = JsonUtility.ToJson(weaponData);

        return jsonData;
    }

    private async void setIPFS(string www)
    {
        currentTime = Time.time;
            metaDataWeapon.weaponData = JsonUtility.FromJson<WeaponData>(www);
            metaDataWeapon.image = "https://bafkreiezrpaxfumy7rbv4234krmboniyyuh5unnved2tf5btgfo7hy76iq.ipfs.nftstorage.link/";
            metaDataWeapon.name = "SweetP Weapon";
            print(metaDataWeapon.weaponData.weapon_id);
        
        string jsonData = JsonUtility.ToJson(metaDataWeapon);
        print("json: "+jsonData);
        NFTStorageUploadResponse uploadResponse = await ImplementNFTStorage.instance.NSC.UploadDataFromJsonHttpClient(jsonData);

        if (uploadResponse != null && uploadResponse.ok)
        {
            string uploadedCID = uploadResponse.value.cid;
            string ipfsUrl = "https://" + uploadedCID + ".ipfs.nftstorage.link/";
            Debug.Log("Uploaded CID: " + uploadedCID);
            StartCoroutine(MintNFT(SmartContractInteraction.userAccount.Address, ipfsUrl));
            // 이제 uploadedCID를 사용하여 IPFS 네트워크에서 데이터를 가져오거나 공유할 수 있습니다.
        }
        else
        {
            Debug.Log("Error uploading JSON data or retrieving CID.");
        }
    }

    public IEnumerator MintNFT(string recipient, string tokenURI)
    {
        yield return StartCoroutine(PPC721Contract.MintNFT(recipient, tokenURI, (Address, ex) =>
        {
            Debug.Log($"MintNFT Contract Address: {Address}");

            currentTime = Time.time - currentTime;
            print(currentTime);
        }));
    }

}