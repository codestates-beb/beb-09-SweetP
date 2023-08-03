using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemSlot : MonoBehaviour
{

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

    public void AddItem(ScrollData _scrollData, int _count =1)
    {
        scrollData = _scrollData;
        itemCount = _count;
        goCountImage.SetActive(true);
        itemCountText.text = itemCount.ToString();

        switch (scrollData.scrollType)
        {
                //sword
            case (ScrollType)0:
                itemImage.sprite = Item.instance.Scroll;
                SetColor(1f, 1f, 1f,1f);
                break;
            //bow
            case (ScrollType)1:
                itemImage.sprite = Item.instance.Scroll;
                SetColor(0.492f, 0.354f, 1f,1f);
                break;
            //magic
            case (ScrollType)2:
                itemImage.sprite = Item.instance.Scroll;
                SetColor(0.717f, 0.224f, 0f,1f);
                break;
        }

        
    }

    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        itemCountText.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    private void ClearSlot()
    {
        scrollData = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0f, 0f, 0f,0f);

        itemCountText.text = "0";
        goCountImage.SetActive(false);

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
