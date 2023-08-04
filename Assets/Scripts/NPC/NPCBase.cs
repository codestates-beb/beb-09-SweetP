using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCBase : MonoBehaviour, INPC
{
    public Transform player;
    private Animator animator;
    private float searchPlayerRadius = 5f;
    private float originalRotationX;
    private Quaternion originalRotation;
    private GameObject ContractPanel;
    private TextMeshProUGUI nameText;

    public NPCData npcData;
    private GameObject ThisNPCPanel;
    public bool IsContract = false;
    public virtual void OnContract()
    {
        ContractPanel.SetActive(false);
        ThisNPCPanel.SetActive(true);
    }


    public virtual void OffContract()
    {
        ThisNPCPanel.SetActive(false);
        UIManager.instance.IsOpenPanel = false;
    }

    private void FindPlayer()
    {
        if(player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if(distanceToPlayer <= searchPlayerRadius)
            {
                animator.SetTrigger("Hello");
                Vector3 currentRotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(originalRotationX, currentRotation.y, currentRotation.z);

                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0f;
                transform.rotation = Quaternion.LookRotation(lookDirection);
                nameText.text = transform.name;
                if(!IsContract)
                ContractPanel.SetActive(true);
                IsContract = true;
            }
            else if(distanceToPlayer > searchPlayerRadius)
            {
                if (IsContract == true)
                {
                    animator.ResetTrigger("Hello");
                    animator.SetTrigger("GoodBye");
                    transform.rotation = originalRotation;
                    ContractPanel.SetActive(false);
                    IsContract = false;
                }
            }
        }
                
        
        
    }


    // Start is called before the first frame update
    void Start()
    {
        ContractPanel = UIManager.instance.ContractPanel;
        nameText = UIManager.instance.ContractNPCNameText;
        originalRotationX = transform.rotation.eulerAngles.x;
        originalRotation = transform.rotation;

        animator = GetComponent<Animator>();
        switch (npcData)
        {
            //battle
            case (NPCData)0:
                ThisNPCPanel = UIManager.instance.BattlePanel;
                break;
            //shop
            case (NPCData)1:
                ThisNPCPanel = UIManager.instance.ShopPanel;
                break;
            //change
            case (NPCData)2:
                ThisNPCPanel = UIManager.instance.MarketPanel;
                break;
            //Upgrade
            case (NPCData)3:
                ThisNPCPanel = UIManager.instance.UpgradePanel;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        if(IsContract == true)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (!UIManager.instance.IsOpenPanel)
                {
                    print("Contract!" + transform.name);
                    OnContract();
                    //ContractPanel.SetActive(false);
                    UIManager.instance.IsOpenPanel = true;

                }
                else if (UIManager.instance.IsOpenPanel)
                {
                    OffContract();
                }
            }         
        }
    }
}
