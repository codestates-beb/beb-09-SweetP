using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadingManager : MonoBehaviour
{
    static string nextScene;

    
    [SerializeField]
    GameObject LoadText;
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneProgress());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {


            yield return null;

            if(op.progress >= 0.9f)
            {
                GameObject canvas = GameObject.Find("Canvas");

                int count = canvas.transform.childCount;
                var remove = new System.Collections.Generic.List<GameObject>();
                for(int i=0; i<count; i++)
                {
                    Transform child = canvas.transform.GetChild(i);
                    if(child.name == "Spinner(Clone)")
                    {
                        remove.Add(child.gameObject);
                    }
                }

                foreach(var spinnerClone in remove)
                {
                    Destroy(spinnerClone);
                }
                LoadText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    op.allowSceneActivation = true;
                }
                //yield break;
            }
        }
    }
}
