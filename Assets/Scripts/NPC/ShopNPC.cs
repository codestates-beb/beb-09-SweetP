using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class ShopNPC : MonoBehaviour
{

    public ItemCost itemCost;
    private ObjectEventSystem eventSystem;
    // Start is called before the first frame update
    private int player_gold;
    private int player_potion;

    public void ButtonCountUp()
    {
        if (player_gold >= itemCost.potion)
        {
            player_potion++;
            player_gold -= itemCost.potion;
            UIManager.instance.ItemCountText.text = player_potion.ToString();
            UIManager.instance.MyGoldText.text = player_gold.ToString();
        }
    }

    public void ButtonCountDown()
    {
        if (player_potion > 0)
        {
            player_potion--;
            player_gold += (itemCost.potion / 5) * 4;
            UIManager.instance.ItemCountText.text = player_potion.ToString();
            UIManager.instance.MyGoldText.text = player_gold.ToString();
        }
    }

    public void ButtonDecide()
    {
        ItemManager.instance.itemData.player_gold = player_gold;
        ItemManager.instance.itemData.player_potion = player_potion;
        UIManager.instance.ShopPanel.SetActive(false);
        UIManager.instance.IsContract = false;
        UIManager.instance.RefreshPotionCount();
    }

    private void OnShopSetActiveChanged(object sender, EventArgs e)
    {
        player_gold = ItemManager.instance.itemData.player_gold;
        player_potion = ItemManager.instance.itemData.player_potion;

        UIManager.instance.ItemCostText.text = itemCost.potion.ToString();
        UIManager.instance.MyGoldText.text = player_gold.ToString();
        UIManager.instance.ItemCountText.text = player_potion.ToString();
    }
    private void Awake()
    {

        eventSystem = UIManager.instance.ShopPanel.GetComponent<ObjectEventSystem>();

        if (eventSystem != null)
        {
            eventSystem.ObjectSetActiveChanged += OnShopSetActiveChanged;
        }

    }
    void Start()
    {
        //player_gold = ItemManager.instance.itemData.player_gold;
        //player_potion = ItemManager.instance.itemData.player_potion;

        //UIManager.instance.ItemCostText.text = itemCost.potion.ToString();
        //UIManager.instance.MyGoldText.text = player_gold.ToString();
        //UIManager.instance.ItemCountText.text = player_potion.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
