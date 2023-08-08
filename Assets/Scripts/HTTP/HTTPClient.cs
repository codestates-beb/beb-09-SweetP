using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class HTTPClient : MonoBehaviour
{
    public static HTTPClient _instance;
    public static HTTPClient instance
    {

        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<HTTPClient>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }

    public GameObject progressSpinner;
    public GameObject spinner;
    public bool IsSpinner =false;

    private void Start()
    {


        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public void StartSpinner()
    {
        if (!IsSpinner)
        {
            IsSpinner = true;
            GameObject canvas = GameObject.Find("Canvas");
            spinner = Instantiate(progressSpinner, canvas.transform);
        }
    }

    public void EndSpinner()
    {
        if (IsSpinner)
        {
            Destroy(spinner);
            IsSpinner = false;
        }
    }

    public void GET(string url, Action<string> callback)
    {
        StartCoroutine(WaitForRequest(url, callback));
    }

    public void POST(string url, string input, Action<string> callback)
    {
        StartCoroutine(WaitForRequest(url,input, callback));
    }

    public void PUT(string url, string input, Action<string> callback)
    {
        StartCoroutine(WaitForPutRequest(url, input, callback));
    }

    public void DELETE(string url, Action<string> callback)
    {
        StartCoroutine(WaitForDeleteRequest(url, callback));
    }


    private IEnumerator WaitForPutRequest(string url, string input, Action<string> callback)
    {

        byte[] bodyRaw = Encoding.UTF8.GetBytes(input);

        using (UnityWebRequest www = new UnityWebRequest(url, "PUT"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            //Destroy(spinner);
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }
    }

    private IEnumerator WaitForRequestTest(string url, Action<string> callback)
    {
        GameObject canvas = GameObject.Find("Canvas");
        spinner = Instantiate(progressSpinner, canvas.transform);

        if (!IsSpinner)
        {
            print("spinnmererer");
            spinner.SetActive(true);
            IsSpinner = true;
        }
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            Destroy(spinner);
            IsSpinner = false;
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }
    }

    private IEnumerator WaitForRequest(string url, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    private IEnumerator WaitForDeleteRequest(string url, Action<string> callback)
    {
        GameObject canvas = GameObject.Find("Canvas");
        spinner = Instantiate(progressSpinner, canvas.transform);

        if (!IsSpinner)
        {
            spinner.SetActive(true);
            IsSpinner = true;
        }
        using (UnityWebRequest www = UnityWebRequest.Delete(url))
        {
            yield return www.SendWebRequest();

            Destroy(spinner);
            IsSpinner = false;
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else if (www.result == UnityWebRequest.Result.Success) // 응답 확인
            {
                callback(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("DELETE request failed with unknown result.");
            }
        }
    }

    private IEnumerator WaitForRequest(string url, string input, Action<string> callback)
    {
        GameObject canvas = GameObject.Find("Canvas");
        //spinner = Instantiate(progressSpinner, canvas.transform);
        //spinner.SetActive(true);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(input);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            //Destroy(spinner);
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }
    }

    
}
