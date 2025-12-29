using UnityEngine;

namespace JO
{
    public static class HotfixEntry
    {
        public static void Init()
        {
            Debug.Log("[HOTFIX] HotfixEntry.Init() v 2");

            // TODO：后面真正的游戏初始化都从这里发车
            // 比如：
            //  - 注册事件
            //  - 初始化各个 Mgr
            //  - 打开第一个 UI 等
        }
    }
}