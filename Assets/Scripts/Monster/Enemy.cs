using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class Enemy : LivingEntity
{
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

    private Animator enemyAnimator;
    private AudioSource enemyAudioPlayer;
    private Renderer enemyRenderer;

    private bool isChase = true;
    private bool isAttack = false;
    public float damage = 10f;
    public float timeBetAttack = 5.5f;
    private float lastAttackTime;

    [Header("Gold")]
    public int dropGold = 0;
    public int dropGoldMin = 10;
    public int dropGoldMax = 25;

    [Header("Drop Scroll")]
    public List<ScrollDropTable> scrollDropTables = new List<ScrollDropTable>();
    
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
        float targetRange =0f;
    RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position,
            targetRadius,
            transform.forward,
            targetRange,
            LayerMask.GetMask("Player"));

        if(rayHits.Length >0 && !isAttack)
        {
            if (Time.time >= lastAttackTime + timeBetAttack)
            {
                enemyAnimator.SetTrigger("Attack");
                isChase = false;
                pathFinder.isStopped = true;
                isAttack = true;
            }
        }
        if(hasTarget && rayHits.Length <= 0 && isChase == false)
        {
            isChase = true;
            pathFinder.isStopped = false;
            //EndAttack();
            Debug.Log("miss");

        }
    }

    private void Awake()
    {
        // 초기화
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();

        enemyRenderer = GetComponentInChildren<Renderer>();
        lastAttackTime = 0f;

        healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        healthBarInstance.transform.SetParent(GameObject.Find("DesertCanvas").transform);

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

        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        Vector3 healthBarPosition = transform.position + new Vector3(0f, 2f, 0f);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(healthBarPosition);
        healthBarInstance.transform.position = screenPosition;

        enemyAnimator.SetBool("HasTarget", hasTarget);
        Targeting();

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
    public override void Die()
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        Collider enemyCollider= GetComponent<Collider>();

        enemyCollider.enabled = false;
        

        pathFinder.isStopped = true;
        pathFinder.enabled = false;

        dropGold = Random.Range(dropGoldMin, dropGoldMax);
        enemyAnimator.SetTrigger("Die");
        enemyAudioPlayer.PlayOneShot(deathSound);
        healthSlider.gameObject.SetActive(false);
        ItemManager.instance.itemData.player_gold += dropGold;
        WeaponManager.instance.WeaponUse(WeaponManager.instance.curruentWeaponData);
        DropScoll();
    }

    void Attack()
    {
        AttackRange.enabled = true;
    }

    void EndAttack()
    {
        isAttack = false;
        AttackRange.enabled = false;

        lastAttackTime = Time.time;
        //Debug.Log("Last Attack : "+lastAttackTime);
    }

    private void DropScoll()
    {
        for(int i=0; i< scrollDropTables.Count; i++)
        {
            if (scrollDropTables[i].dropProb != 0)
            {
                if (Random.value < (scrollDropTables[i].dropProb / 100))
                {
                    switch (scrollDropTables[i].scrollType)
                    {
                        case (ScrollType)0:
                            ItemManager.instance.scrollDataList[0].count++;
                            break;
                        case (ScrollType)1:
                            ItemManager.instance.scrollDataList[1].count++;
                            break;
                        case (ScrollType)2:

                            ItemManager.instance.scrollDataList[2].count++;
                            break;

                    }
                }
            }

        }
    }

}
