using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldExchange : MonoBehaviour
{
    public Transform mainUI;
    public string swapSymbol;
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enter() {
        mainUI.localPosition = Vector2.zero;
    }

    public void Exit() {
        mainUI.localPosition = Vector3.up * 80000;
    }


}
