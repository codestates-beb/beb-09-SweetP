using System;
using UnityEngine;
public class ObjectEventSystem : MonoBehaviour
{
    public event EventHandler ObjectSetActiveChanged;

    public void SetOnbjectSetActive(bool active)
    {
        gameObject.SetActive(active);

        OnObjectSetActiveChanged();
    }

    protected virtual void OnObjectSetActiveChanged()
    {
        ObjectSetActiveChanged?.Invoke(this, EventArgs.Empty);
    }
}
