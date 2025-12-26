using UnityEngine;

namespace JO
{
    public class ResMgr
    {
        private const string IMGPATH = "Art/Dynamic/";

        private GameObject LoadPrefab(string path)
        {
            GameObject prefab = LoadAssets<GameObject>(path + ".prefab", ResType.Prefab);
            return prefab;
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
            return LoadAssets<Sprite>(path+".png", ResType.Sprite);//todo待扩展 不是所有的都是png
        }

        public T LoadGameDataN<T>(string name) where T : ScriptableObject
        {
            string path = "Configs/" + name + ".asset";
            return LoadAssets<T>(path, ResType.Asset);
        }

        public GameObject InstantiateObj(string path, Transform parent = null)
        {
            GameObject obj = LoadPrefab(path);
            if (obj == null)
            {
                Debug.LogError("obj is null, the path : " + path);
                return new GameObject();
            }
            return Object.Instantiate<GameObject>(obj, parent);
        }
        public GameObject InstantiateLoadObj(string path, Transform parent = null)
        {
            return InstantiateObj(path, parent);
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
}