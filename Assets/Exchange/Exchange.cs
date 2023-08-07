using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exchange : MonoBehaviour
{
    public Transform exchangeEnterUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B)) {
            exchangeEnterUI.localPosition = Vector2.zero;
        }
    }

    public void Exit() {
        exchangeEnterUI.localPosition = Vector2.up * 80000;
    }
}
