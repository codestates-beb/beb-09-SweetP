using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    public float damage;
    public Vector3 pos;
    public float speed = 5f;
    public Transform playerTransform;
    private Rigidbody rigidbody;
    private Vector3 direction;
    private float distanceThisFrame;

    private bool hasFire = false;
    private void OnTriggerEnter(Collider other)
    {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행

        LivingEntity attackTarget = other.GetComponent<LivingEntity>();
        if (attackTarget != null && attackTarget.CompareTag("Monster"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - other.transform.position;

            attackTarget.OnDamage(damage, hitPoint, hitNormal);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        direction = playerTransform.forward;
        distanceThisFrame = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasFire)
        {
            rigidbody.velocity = direction * speed;
            hasFire = true;
        }

        

    }
}
