using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SoundManager : MonoBehaviour
{

    private static SoundManager _instance;
    public static SoundManager instance
    {

        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<SoundManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    public AudioSource audioSource;
    public AudioClip buttonClick;
    public List<Button> buttons = new List<Button>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSeneLoaded;

        audioSource = GetComponent<AudioSource>();
        //InitializeButtons();

    }

    private void Update()
    {
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                if (HTTPClient.instance.IsSpinner)
                {
                    button.enabled = false;
                }
                else
                {
                    button.enabled = true;
                }
            }
        }
    }
    public void InitializeButtons()
    {
        Button[] foundButtons = FindObjectsOfType<Button>();
        buttons.Clear();
        buttons.AddRange(foundButtons);

        foreach(Button button in buttons)
        {
            button.onClick.AddListener(() => PlayerCLickSound());
        }
    }

    private void PlayerCLickSound()
    {
        audioSource.PlayOneShot(buttonClick);
        InitializeButtons();
    }

    private void OnSeneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeButtons();
    }
}
