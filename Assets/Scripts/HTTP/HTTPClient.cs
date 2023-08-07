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



    public void GET(string url, Action<string> callback)
    {
        StartCoroutine(WaitForRequest(url, callback));
        print("Get" + url);
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
        GameObject canvas = GameObject.Find("Canvas");
        //spinner = Instantiate(progressSpinner, canvas.transform);
        //spinner.SetActive(true);
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

    public IEnumerator WaitForRequest(string url, Action<string> callback)
    {
        GameObject canvas = GameObject.Find("Canvas");
        spinner = Instantiate(progressSpinner, canvas.transform);

        spinner.SetActive(true);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            Destroy(spinner);
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

    public IEnumerator WaitForDeleteRequest(string url, Action<string> callback)
    {
        GameObject canvas = GameObject.Find("Canvas");
        spinner = Instantiate(progressSpinner, canvas.transform);

        spinner.SetActive(true);
        using (UnityWebRequest www = UnityWebRequest.Delete(url))
        {
            yield return www.SendWebRequest();

            Destroy(spinner);
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
