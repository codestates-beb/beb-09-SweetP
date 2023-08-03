using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public Vector3 pos;
    public float speed = 5f;
    private void OnTriggerEnter(Collider other)
    {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행

        LivingEntity attackTarget = other.GetComponent<LivingEntity>();
        if (attackTarget != null && attackTarget.CompareTag("Player"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - other.transform.position;

            attackTarget.OnDamage(damage, hitPoint, hitNormal);
        }

    }

    public void SetTarget(Vector3 targetPosition)
    {
        this.pos = targetPosition;
    }

    private void Fire()
    {



    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어를 향해 이동
        Vector3 direction = pos - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // 목표 위치와의 거리가 일정 값보다 작으면 총알을 삭제
        if (direction.magnitude <= distanceThisFrame)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);

    }


}
