using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LDBaseMono : MonoBehaviour
{
    protected static long MissFileCode = 404;
    public void Load(string url, Action<string, bool, long> OnResult, int timeout = -1)
    {
        StartCoroutine(LoadWebResText(url, OnResult, timeout));
    }
    public void LoadBytes(string url, Action<byte[], bool, long> OnResult)
    {
        StartCoroutine(LoadWebResBytes(url, OnResult));
    }
    public void LoadPost(string url, string jsondata, Action<string, bool, long> OnResult, int timeout = -1)
    {
        StartCoroutine(LoadRequestPost(url, jsondata, OnResult, timeout));
    }
    protected void FreshVersion(string tag,string verson)
    {
        Transform versionTsf = transform.Find("MainCanvas/Version");
        if(versionTsf != null)
        {
            Text text = versionTsf.GetComponent<Text>();
            if(text != null)
            {
                string newStr = tag + " " + verson;
                Debug.Log("V == " + newStr);
                text.text = newStr;
            }
        }
    }
    IEnumerator LoadRequestPost(string url, string jsondata, Action<string, bool, long> OnResult, int timeOut)
    {
        Debug.Log(" MainUpdate  LoadWebResText url  " + url);
        Debug.Log(" MainUpdate  LoadWebResText data  " + jsondata);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.Default.GetBytes(jsondata));
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-Type", "application/json");
            if (timeOut > 0)
            {
                www.timeout = timeOut;
            }
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.responseCode + " error " + www.error + " url " + url);
                OnResult(string.Empty, false, www.responseCode);
            }
            else
            {
                OnResult(www.downloadHandler.text, true, 0);
            }
        }
    }
    IEnumerator LoadWebResBytes(string url, Action<byte[], bool, long> OnResult)
    {
        Debug.Log(" MainUpdate  LoadWebResText  " + url);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.responseCode + " error " + www.error + " url " + url);
                OnResult(null, false, www.responseCode);
            }
            else
            {
                OnResult(www.downloadHandler.data, true, 0);
            }
        }
    }
    IEnumerator LoadWebResText(string url, Action<string, bool, long> OnResult, int timeout = -1)
    {
        Debug.Log(" MainUpdate  LoadWebResText  " + url);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            if (timeout > 0)
            {
                www.timeout = timeout;
            }
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.responseCode + " error " + www.error + " url " + url);
                OnResult(string.Empty, false, www.responseCode);
            }
            else
            {
                OnResult(www.downloadHandler.text, true, 0);
            }
        }
    }
}