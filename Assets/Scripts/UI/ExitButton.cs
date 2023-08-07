using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public GameObject ExitTarget;

    public void Exit()
    {
        ExitTarget.SetActive(false);
    }
}
