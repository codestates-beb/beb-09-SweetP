
using System;
using UnityEngine;

public class DisableEvent : MonoBehaviour
{
    private ObjectEventSystem eventSystem;

    private void Awake()
    {
        eventSystem = GetComponent<ObjectEventSystem>();
    }

    private void OnDisable()
    {
        if(eventSystem != null)
        {
            eventSystem.SetOnbjectSetActive(false);
        }
    }
}
