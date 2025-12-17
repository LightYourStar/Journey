using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JO
{
    public class LDGameDataCfg
    {
        public string DataName;
        public string DataKey = null;
    }
    public partial class GameDatas
    {
        private Dictionary<string, ScriptableObject> AllData = new Dictionary<string, ScriptableObject>(256);
        private List<LDGameDataCfg> m_WaitLoadData = new List<LDGameDataCfg>(256);

        public List<int> InnerData = new List<int>(256);

        public GameDatas()
        {
            LoadGameData();
            for (int i = 0; i < 256; i++)
            {
                InnerData.Add(i);
            }
        }


        private void LoadGameData()
        {
            AddLoadData(GlobalCfg.CfgName);
            AddLoadData(Dialogue.CfgName);
            AddLoadData(EndShow.CfgName);

            AddLoadData(LetterMain.CfgName);
            AddLoadData(LetterColumn.CfgName);
            AddLoadData(LetterPiece.CfgName);
            AddLoadData(LetterOutCome.CfgName);
        }
        public IEnumerator WaitOnInitSucceed()
        {
            foreach (LDGameDataCfg item in m_WaitLoadData)
            {
                LoadGameDataImp(item.DataName, item.DataKey);
            }
            //判断资源是否加载完成
            // LoadFont();
            yield return null;
        }

        private void LoadGameDataImp(string cfgName, string key = null)
        {
            ScriptableObject data = Global.gApp.gResMgr.LoadGameDataN<ScriptableObject>(cfgName);
            if (key == null)
            {
                AllData[cfgName] = data;
            }
            else
            {
                AllData[key] = data;
            }
        }
        private void AddLoadData(string cfgName, string key = null)
        {
            LDGameDataCfg gameDataCfg = new LDGameDataCfg();
            gameDataCfg.DataName = cfgName;
            gameDataCfg.DataKey = key;
            if (key == null)
            {
                gameDataCfg.DataKey = cfgName;
            }
            m_WaitLoadData.Add(gameDataCfg);
        }
        public T GetData<T>(string key) where T : ScriptableObject
        {
            return AllData[key] as T;
        }
        public void OnDestroy()
        {
        }
    }
}