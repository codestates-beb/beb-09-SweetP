using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemSlot : MonoBehaviour
{
    public ScrollType scrollType;
    public ScrollData scrollData;
    public Image itemImage;
    public int itemCount;
    private ItemSlot itemSlot;

    [SerializeField]
    private TextMeshProUGUI itemCountText;
    [SerializeField]
    private GameObject goCountImage;

    private void SetColor(float r, float g, float b, float a)
    {
        Color color = itemImage.color;
        color.r = r;
        color.g = g;
        color.b = b;
        color.a = a;
        itemImage.color = color;
    }

    public void SetItem()
    {
        goCountImage.SetActive(true);
        itemImage.sprite = Item.instance.Scroll;
        switch (scrollType)
        {
                //sword
            case (ScrollType)0:
                SetColor(1f, 1f, 1f,1f);
                itemCountText.text = ItemManager.instance.scrollData.normal.ToString();
                
                break;
            //bow
            case (ScrollType)1:
                SetColor(0.492f, 0.354f, 1f,1f);
                itemCountText.text = ItemManager.instance.scrollData.unique.ToString();
                break;
            //magic
            case (ScrollType)2:
                SetColor(0.717f, 0.224f, 0f,1f);
                itemCountText.text = ItemManager.instance.scrollData.legendary.ToString();
                break;
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        itemSlot = gameObject.GetComponent<ItemSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
