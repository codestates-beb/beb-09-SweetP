using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Scroll : MonoBehaviour
{
    public ScrollType scrollType;
    public int IncreseProb = 0;

    private void Start()
    {
        switch (scrollType)
        {
            case (ScrollType)0:
                IncreseProb = 0;
                break;
            case (ScrollType)1:
                IncreseProb = 10;
                break;
            case (ScrollType)2:
                IncreseProb = 20;
                break;
        }
    }
}
