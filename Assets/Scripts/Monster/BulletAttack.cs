using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : MonoBehaviour
{

    public Bullet BulletPrefab;
    public Transform BulletFirePosition;
    public float damage;
    private Enemy enemy;
    public void Shoot()
    {
        Bullet bullet = BulletPrefab;
        bullet.damage = damage;
        Vector3 pos = BulletFirePosition.position;
        if (enemy.targetEntity != null && !enemy.targetEntity.dead)
            bullet.SetTarget(enemy.targetEntity.transform.position);
        Instantiate(bullet, pos, Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.GetComponent<Enemy>();
        damage = enemy.damage;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
