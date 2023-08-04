using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MarketSlot : MonoBehaviour
{
    public WeaponData weaponData;
    public MarketData marketData;
    public Image itemImage;
    private MarketSlot marketSlot;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI natureText;
    public TextMeshProUGUI durText;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;

    public void InitSlot(WeaponData _weaponData, MarketData _marketData)
    {
        weaponData = _weaponData;
        marketData = _marketData;
        switch (weaponData.weapon_type)
        {
            //sword
            case 0:
                itemImage.sprite = Item.instance.Sword;
                break;
            //bow
            case 1:
                itemImage.sprite = Item.instance.Bow;
                break;
            //magic
            case 2:
                itemImage.sprite = Item.instance.Magic;
                break;
        }

        levelText.text = weaponData.weapon_upgrade.ToString();
        healthText.text = weaponData.weapon_hp.ToString();
        damageText.text = weaponData.weapon_atk.ToString();
        natureText.text = weaponData.weapon_element.ToString();
        durText.text = weaponData.weapon_durability.ToString();

        nameText.text = marketData.weapon_owner.ToString();
        costText.text = marketData.weapon_cost.ToString();
        
    }

    public void OpenBuyPanel()
    {
        MarketNPC.instance.selectMarket = marketSlot;
        UIManager.instance.BuyPanel.transform.position = transform.position;
        UIManager.instance.BuyPanel.SetActive(true);
    }

    

    // Start is called before the first frame update
    void Start()
    {
        marketSlot = gameObject.GetComponent<MarketSlot>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
