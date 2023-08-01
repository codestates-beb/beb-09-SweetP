using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCBase : MonoBehaviour, INPC
{
    public Transform player;
    private float searchPlayerRadius = 3f;
    private float originalRotationX;
    private Quaternion originalRotation;
    private GameObject ContractPanel;
    private TextMeshProUGUI nameText;
    private bool IsContract = false;
    private bool IsOpenPanel = false;

    public NPCData npcData;
    private GameObject ThisNPCPanel;
    
    public virtual void OnContract()
    {
        ThisNPCPanel.SetActive(true);
    }


    public virtual void OffContract()
    {
        ThisNPCPanel.SetActive(false);
    }

    private void FindPlayer()
    {
        if(player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if(distanceToPlayer <= searchPlayerRadius)
            {
                Vector3 currentRotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(originalRotationX, currentRotation.y, currentRotation.z);

                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0f;
                transform.rotation = Quaternion.LookRotation(lookDirection);
                nameText.text = transform.name;
                ContractPanel.SetActive(true);
                IsContract = true;
            }
            else
            {
                if (IsContract == true)
                {


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
                ThisNPCPanel = UIManager.instance.ChangePanel;
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
                if (!IsOpenPanel)
                {
                    print("Contract!" + transform.name);
                    OnContract();
                    IsOpenPanel = true;

                }
                else
                {
                    OffContract();
                }
            }         
        }
    }
}
