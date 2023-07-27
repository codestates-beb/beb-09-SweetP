using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class PlayerTB
{
    public int player_id;
    public string player_address;
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
}