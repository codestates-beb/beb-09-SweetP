using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    public WeaponData weaponData;
    // Start is called before the first frame update
    void Start()
    {
        weaponData = new WeaponData();
        weaponData = WeaponManager.instance.curruentWeaponData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        LivingEntity attackTarget = other.GetComponent<LivingEntity>();
        if (attackTarget != null && other.CompareTag("Monster"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - other.transform.position;

            attackTarget.OnDamage(weaponData.weapon_atk + (weaponData.weapon_upgrade * 20) , hitPoint, hitNormal);
            Debug.Log("Attack");
        }
    }
}
