using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoad : MonoBehaviour
{
    private static UILoad _instance;
    public static UILoad instance
    {

        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<UILoad>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
