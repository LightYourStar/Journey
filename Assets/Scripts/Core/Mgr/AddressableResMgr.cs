using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JO
{
    public class AddressableResMgr : ResMgr
    {
        public AddressableResMgr()
        {

        }

        public override U LoadAssets<U>(string path, ResType resType)
        {
            try
            {
                if (path == string.Empty)
                {
                    Debug.LogError(" 加载资源失败1 ，资源路径名称 没配置 " + path);
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                    return null;
                }

                AsyncOperationHandle<U> operationHandle = Addressables.LoadAssetAsync<U>("Assets/ResBundle/"+path);
                if (operationHandle.Status == AsyncOperationStatus.Failed)
                {
                    Debug.LogError(" 加载资源失败，请导出资源 " + path);
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                    return null;
                }
                return operationHandle.WaitForCompletion();
            }
            catch (System.Exception e)
            {
                Debug.LogError("exception " + e.Message);
                Debug.LogError(" 加载资源异常 " + path);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                return null;
            }


        }
    }
}