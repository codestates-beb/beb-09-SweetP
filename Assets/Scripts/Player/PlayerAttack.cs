using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private static PlayerAttack _instance;
    public static PlayerAttack instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<PlayerAttack>();
                
            }
            return _instance;
        }
    }
    public bool IsWeaponEquip = false;
    public Transform pivotWeaponR;
    public BoxCollider colliderWeapon;
    public GameObject objWeapon;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        
    }

    private void AttackColliderOn()
    {
        switch(WeaponManager.instance.curruentWeaponData.weapon_type)
        {
            case 0:
                break;

            case 1:
                break;

            case 2:
                objWeapon.GetComponent<Staff>().Shoot();
                break;

        }
        AudioSource.PlayClipAtPoint(Player.instance.attackClip, transform.position);

        colliderWeapon.enabled = true;
        
    }

    private void AttackColliderOff()
    {
        colliderWeapon.enabled = false;
    }


}
