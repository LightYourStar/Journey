using System.Collections.Generic;
using UnityEngine;

namespace JO
{
    public class ResMgr
    {
        protected Dictionary<ResSceneType, Dictionary<string, GameObject>> m_CachePrefabs = new Dictionary<ResSceneType, Dictionary<string, GameObject>>();


        private const string IMGPATH = "Art/Dynamic/";

        private GameObject LoadPrefab(string path, ResSceneType resSceneType)
        {
            Dictionary<string, GameObject> prefabs = m_CachePrefabs[resSceneType];
            if (prefabs.ContainsKey(path))
            {
                GameObject prefab = prefabs[path];
                return prefab;
            }
            else
            {
                GameObject prefab = LoadAssets<GameObject>(path, ResType.Prefab);
                prefabs.Add(path, prefab);
                return prefab;
            }
        }

        public virtual void UnLoadAssets()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public virtual U LoadAssets<U>(string path, ResType resType) where U : UnityEngine.Object
        {
            return Resources.Load<U>(path);
        }

        public Sprite LoadSprite(string name)
        {
            string path = IMGPATH + name;
            return LoadAssets<Sprite>(path, ResType.Sprite);
        }

        public T LoadGameDataN<T>(string name) where T : ScriptableObject
        {
            string path = "Configs/" + name;
            return LoadAssets<T>(path, ResType.Asset);
        }

        public GameObject InstantiateObj(string path,ResSceneType resSceneType, Transform parent = null)
        {
            GameObject obj = LoadPrefab(path, resSceneType);
            if (obj == null)
            {
                Debug.LogError("obj is null, the path : " + path);
                return new GameObject();
            }
            return Object.Instantiate<GameObject>(obj, parent);
        }
        public GameObject InstantiateLoadObj(string path, ResSceneType resSceneType, Transform parent = null)
        {
            return InstantiateObj(path, resSceneType, parent);
        }

        public void DestroyGameObj(GameObject go)
        {
            if (go != null)
            {
                go.SetActive(false);
                GameObject.Destroy(go);
            }
        }
    }

    public enum ResType
    {
        Prefab = 0,//预制
        SpriteAtlas,//图集
        Sprite,//图片
        Clip,//音效
        Font,//字体
        Asset,//unity资源
        Material,//unity资源
        Texture,//图片
    }

    public enum ResSceneType
    {
        NormalRes = 1,//
        NormalUI = 2,// 普通ui
        Resident = 101,// 常驻资源。不卸载 且 只能自己关的ui
    }
}