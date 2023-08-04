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
    public int weapon_type;
    public int weapon_unique;
    public int weapon_atk;
    public int weapon_hp;
    public WeaponNature weapon_element;
    public int weapon_durability;
    public int weapon_upgrade;
    public int weapon_sale;
}

[Serializable]
public class ItemData
{
    public int player_id;
    public int player_gold;
    public int player_potion;
}

[Serializable]
public class MonsterData
{
    public MonsterType monsterType;
    public Transform spawnerPos;
    public float baseDamage = 20f;
    public float baseHP = 100f;
    public float baseSpeed = 1f;
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