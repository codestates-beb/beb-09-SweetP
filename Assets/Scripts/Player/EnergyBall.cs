using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    public float damage;
    public WeaponNature element;
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
        element = WeaponManager.instance.curruentWeaponData.weapon_element;
        UpgradeParticle();
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


    public void UpgradeParticle()
    {
        switch (element)
        {
            case WeaponNature.Fire:
                GameObject particleFire = Instantiate(WeaponManager.instance.staffParticle[0], transform);
                break;
            case WeaponNature.Water:
                GameObject particleWater = Instantiate(WeaponManager.instance.staffParticle[1], transform);
                break;
            case WeaponNature.Thunder:
                GameObject particleThunder = Instantiate(WeaponManager.instance.staffParticle[2], transform);
                break;
            case WeaponNature.Earth:
                GameObject particleEarth = Instantiate(WeaponManager.instance.staffParticle[3], transform);
                break;
        }
    }
}
