using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class PlayerTB
{
    public int player_id;
    public string player_address;
    public string player_name;
}

public class PlayerData
{
    public int player_id;
    public int player_gold;
    public int player_potion;
}

public class PlayerRecord
{
    public int player_id;
    public int player_score;
}

[Serializable]
public class WeaponTB
{
    public int weapon_id;
    public int weapon_owner;
}

[Serializable]
public class WeaponData
{
    public int weapon_id;
    [HideInInspector]
    public int weapon_type =0;
    public int weapon_unique = 0;
    public int weapon_atk = 20;
    public int weapon_hp =20 ;
    public WeaponNature weapon_element = WeaponNature.None;
    public int weapon_durability = 100;
    public int weapon_upgrade = 0;
    public int weapon_sale =0;
}

[Serializable]
public class WeaponDropTable
{
    public int weapon_type;
    public float dropProb;
    public float dropWeaponUpgrade;
}
[Serializable]
public class ItemData
{
    public int player_id;
    public int player_gold =0;
    public int player_potion =0;
}

[Serializable]
public class MonsterData
{
    public MonsterType monsterType;
    public Transform spawnerPos;
    public float baseDamage = 20f;
    public float baseHP = 100f;
    public float baseSpeed = 1f;
    public bool IsAlive = false;
    public Enemy currentEnemyInstance;
    public MonsterData(Transform pos)
    {
        spawnerPos = pos;
    }
}

[Serializable]
public class ItemCost
{
    public int potion = 120;
}

[Serializable]
public class ScrollData
{
    public ScrollType scrollType;
    public int count;
    public int IncreseProb;
}

[Serializable]
public class ScrollDropTable
{
    public ScrollType scrollType;
    public float dropProb;
}

[Serializable]
public class MarketData
{
    public int weapon_id;
    public int weapon_cost;
    public int weapon_owner;
}

public class WeaponSale
{
    public int weapon_Sale;
}

public class MetaDataWeapon
{
    public string image;
    public string name;
    public WeaponData weaponData;
    
}