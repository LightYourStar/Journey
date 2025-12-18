using System;
using System.Collections;
using UnityEngine;

public class DelayedCallback
{
    private Coroutine m_Coroutine;
    private static MonoBehaviour s_Host;

    /// <summary>
    /// 确保有一个主线程宿主
    /// </summary>
    private static MonoBehaviour GetHost()
    {
        if (s_Host != null) return s_Host;

        var go = new GameObject("[DelayedCallback]");
        UnityEngine.Object.DontDestroyOnLoad(go);
        s_Host = go.AddComponent<DelayedCallbackHost>();
        return s_Host;
    }

    public void SetTimeout(TimeSpan delay, Action callback)
    {
        Cancel();

        m_Coroutine = GetHost().StartCoroutine(DelayCoroutine(delay, callback));
    }

    public void Cancel()
    {
        if (m_Coroutine != null)
        {
            GetHost().StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
    }

    private IEnumerator DelayCoroutine(TimeSpan delay, Action callback)
    {
        yield return new WaitForSeconds((float)delay.TotalSeconds);
        callback?.Invoke(); //一定在主线程
    }

    /// <summary>
    /// 空宿主
    /// </summary>
    private class DelayedCallbackHost : MonoBehaviour { }
}