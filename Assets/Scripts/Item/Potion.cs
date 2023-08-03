using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public int potion_count = 0;
    private const int potion_heal = 20;
    public void UsePotion()
    {
        //max hp DDDD;;


        if (ItemManager.instance.itemData.player_potion > 0)
        {
            Player.instance.RestoreHealth(potion_heal);
            ItemManager.instance.itemData.player_potion--;
            UIManager.instance.RefreshPotionCount();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        potion_count = ItemManager.instance.itemData.player_potion;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
