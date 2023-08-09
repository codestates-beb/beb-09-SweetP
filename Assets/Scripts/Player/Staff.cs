using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{

    public EnergyBall EnergyBallPrefab;
    public Transform BulletFirePosition;
    public GameObject playerStartTransform;
    public Transform playerParentTransform;
    public Transform playerTransform;
    public float damage;
    private WeaponData weaponData;

    public void Shoot()
    {
        EnergyBall energyBall = EnergyBallPrefab;
        energyBall.playerTransform = playerTransform;
        energyBall.damage = damage;
        Vector3 pos = BulletFirePosition.position;
        Instantiate(EnergyBallPrefab, pos, Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start()
    {
        weaponData = WeaponManager.instance.curruentWeaponData;

        playerStartTransform = GameObject.Find("PlayerStartPosition");
        playerParentTransform = playerStartTransform.transform.Find("PlayerParent(Clone)");
        playerTransform = playerParentTransform.Find("Player");

        
        damage = weaponData.weapon_atk;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
