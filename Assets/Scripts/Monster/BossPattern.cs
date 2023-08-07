using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern : MonoBehaviour
{
    private Animator animator;
    private Enemy enemy;

    public float attackAngle = 60f;
    public float attackDistance = 5f;
    public float attackRange = 5f;
    public float patternOneDamage = 60f;
    public float patternTwoDamage = 90f;
    public float warningDuration = 3f;
    public Color warningColor;

    private bool isPlayerInPattern = false;

    private GameObject wedgeObject;
    private MeshFilter wedgeMeshFilter;
    private Mesh redWedgeMesh;

    private GameObject circleObject;
    private MeshFilter circleMeshFilter;
    private Mesh redCircleMesh;

    private float currentTime;
    private int score;
    private void Start()
    {
        enemy = gameObject.GetComponent<Enemy>();
        enemy.isBoss = true;
        enemy.currentTimeForScore = Time.time;

        animator = GetComponent<Animator>();

        warningColor = new Color(1f, 0f, 0f, 0.5f);
        circleObject = new GameObject("CircleObject");
        circleObject.transform.SetParent(transform.parent); // 보스의 부모의 자식 오브젝트로 설정
        circleObject.transform.localPosition = Vector3.zero;

        MeshRenderer circleMeshRenderer = circleObject.AddComponent<MeshRenderer>();
        circleMeshFilter = circleObject.AddComponent<MeshFilter>();
        circleMeshRenderer.material.color = warningColor;

        wedgeObject = new GameObject("WedgeObject");
        wedgeObject.transform.SetParent(transform.parent); // 보스의 부모의 자식 오브젝트로 설정
        wedgeObject.transform.localPosition = Vector3.zero;

        MeshRenderer wedgeMeshRenderer = wedgeObject.AddComponent<MeshRenderer>();
        wedgeMeshFilter = wedgeObject.AddComponent<MeshFilter>();
        wedgeMeshRenderer.material.color = warningColor;

        redCircleMesh = CreateRedCircleMesh(attackRange);
        redWedgeMesh = CreateWedgeMesh(attackAngle, attackDistance);

        StartCoroutine(StartRandomPattern());

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    UpdateWedgeMesh();
        //    CheckPlayerInWedgeMesh();
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    UpdateCircleMesh();
        //    CheckPlayerInCircleMesh();
        //}
    }

    private IEnumerator StartRandomPattern()
    {
        while (!enemy.dead)
        {
            //float randomTime = Random.Range(5f, 15f); // 5초에서 15초 사이의 랜덤한 시간을 지정
            yield return new WaitForSeconds(5f);
            enemy.CantAction = true;
            // 패턴을 랜덤으로 선택하여 실행
            int randomPattern = Random.Range(1, 3); // 1 또는 2 중 랜덤한 패턴을 선택
            switch (randomPattern)
            {
                case 1:
                    StartCoroutine(PatternOne());
                    break;
                case 2:
                    StartCoroutine(PatternTwo());
                    break;
                default:
                    Debug.LogError("Invalid pattern number: " + randomPattern);
                    break;
            }
        }
    }

    private IEnumerator PatternOne()
    {
        UpdateWedgeMesh();
        print("charge1");
        animator.SetTrigger("Charge1");
        // 패턴 1 실행: 부채꼴 범위 그리기

        

        // 대기 시간
        yield return new WaitForSeconds(warningDuration);

        animator.SetTrigger("Pattern1");
        print("Patter1!");
        CheckPlayerInWedgeMesh();
        // 플레이어가 여전히 범위 안에 있는지 확인하여 데미지를 입히기
        if (isPlayerInPattern)
        {
            InflictDamageToPlayer(patternOneDamage);
        }

        // 범위 비활성화
        wedgeObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        enemy.CantAction = false;
    }

    private IEnumerator PatternTwo()
    {
        UpdateCircleMesh();
        print("charge2");
        animator.SetTrigger("Charge2");

        // 패턴 2 실행: 원ㅎ 범위 그리기
        
        

        // 대기 시간
        yield return new WaitForSeconds(warningDuration);
        animator.SetTrigger("Pattern2");
        print("Patter1!");
        CheckPlayerInCircleMesh();
        // 플레이어가 여전히 범위 안에 있는지 확인하여 데미지를 입히기

        if (isPlayerInPattern)
        {
            InflictDamageToPlayer(patternTwoDamage);
        }

        // 범위 비활성화
        circleObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        enemy.CantAction = false;
    }

    private Mesh CreateWedgeMesh(float angle, float distance)
    {
        Mesh wedgeMesh = new Mesh();

        int vertexCount = 30;
        Vector3[] vertices = new Vector3[vertexCount + 1];
        int[] triangles = new int[vertexCount * 3];

        // 원점 추가
        vertices[0] = Vector3.zero;

        // 부채꼴의 정점 추가
        for (int i = 0; i < vertexCount; i++)
        {
            float angleStep = angle / vertexCount;
            float currentAngle = -angle / 2f + angleStep * i;
            Vector3 direction = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward;
            vertices[i + 1] = direction * distance;
        }

        // 삼각형 인덱스 설정
        for (int i = 0; i < vertexCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        triangles[vertexCount * 3 - 1] = 1; // 마지막 삼각형 인덱스 수정

        wedgeMesh.vertices = vertices;
        wedgeMesh.triangles = triangles;

        return wedgeMesh;
    }

    private Mesh CreateRedCircleMesh(float radius)
    {
        Mesh circleMesh = new Mesh();

        int vertexCount = 30;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 1) * 3];

        for (int i = 0; i < vertexCount; i++)
        {
            float angle = i * 360f / vertexCount;
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            vertices[i] = new Vector3(x, 0f, z);
        }

        for (int i = 0; i < vertexCount - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        triangles[(vertexCount - 2) * 3] = 0;
        triangles[(vertexCount - 2) * 3 + 1] = vertexCount - 1;
        triangles[(vertexCount - 2) * 3 + 2] = 1;

        circleMesh.vertices = vertices;
        circleMesh.triangles = triangles;

        return circleMesh;
    }


    private void UpdateWedgeMesh()
    {
        Vector3 playerPosition = GetPlayerPosition();
        Vector3 directionToPlayer = playerPosition - transform.position;
        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        // 부채꼴을 보스 위치에 생성하고 플레이어 방향으로 펼치도록 설정합니다.
        wedgeObject.transform.position = transform.position + new Vector3(0,0.5f,0);
        wedgeObject.transform.rotation = Quaternion.Euler(rotationToPlayer.eulerAngles.x, rotationToPlayer.eulerAngles.y, rotationToPlayer.eulerAngles.z);
        wedgeMeshFilter.mesh = redWedgeMesh;

        wedgeObject.SetActive(true);
    }

    private void UpdateCircleMesh()
    {
        Vector3 playerPosition = GetPlayerPosition();
        circleObject.transform.position = playerPosition + new Vector3(0, 0.5f, 0);
        circleMeshFilter.mesh = redCircleMesh;

        circleObject.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
        // 원형 Mesh 업데이트
        //Mesh circleMesh = CreateRedCircleMesh(attackRange);

        circleObject.SetActive(true);

    }

    private IEnumerator DisableObjectAfterDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    private Vector3 GetPlayerPosition()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    private float GetPlayerRotation()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.eulerAngles.y;
    }


    private void CheckPlayerInWedgeMesh()
    {
        Vector3 playerPosition = GetPlayerPosition();
        float distanceToPlayer = Vector3.Distance(wedgeObject.transform.position, playerPosition);
        Vector3 distance = playerPosition - wedgeObject.transform.position;

        isPlayerInPattern = CheckIsInWedgeRange(distance, wedgeObject.transform.forward);

        print(isPlayerInPattern);
    }

    private void CheckPlayerInCircleMesh()
    {
        Vector3 playerPosition = GetPlayerPosition();
        Vector3 distance = playerPosition - circleObject.transform.position;

        isPlayerInPattern = CheckIsInCircleRange(distance);
        print(isPlayerInPattern);
    }


    private bool CheckIsInWedgeRange(Vector3 directionToPlayer, Vector3 from)
    {
        float angleToPlayer = Vector3.Angle(from, directionToPlayer);

        if (angleToPlayer <= attackAngle * 0.5f && directionToPlayer.magnitude <= attackDistance)
        {
            print("Yes");
            return true;
        }

        print("No");
        return false;
    }

    

    private bool CheckIsInCircleRange(Vector3 directionToPlayer)
    {
        
        if (directionToPlayer.magnitude <= attackRange)
        {
            print("Yes");
            return true;
        }
        print("No");
        return false;
    }


    private void InflictDamageToPlayer(float damage)
    {
        // 데미지를 입히는 기능을 구현 (예: 플레이어의 Health 컴포넌트를 조작)
        // 예시) 플레이어가 Health 컴포넌트를 가지고 있다고 가정
        LivingEntity playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<LivingEntity>();
        if (playerEntity != null)
        {
            playerEntity.OnDamage(damage, Vector3.zero, Vector3.zero); // 데미지를 입히는 함수에 데미지 값 전달
        }
    }
}
