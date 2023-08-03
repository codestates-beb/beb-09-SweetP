using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableEvent : MonoBehaviour
{
    private ObjectEventSystem eventSystem;

    private void Awake()
    {
        eventSystem = GetComponent<ObjectEventSystem>();
    }

    private void OnEnable()
    {
        if (eventSystem != null)
        {
            eventSystem.SetOnbjectSetActive(true);
        }
    }
}
