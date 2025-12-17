using JO;
using JO.UIManager;
using System.Collections;
using System.Collections.Generic;
using UIManager;
using UnityEngine;

public enum RolePos
{
    Left,
    Right
}

public class DialogueMgr
{

    private static DialogueMgr _instance;

    public static DialogueMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DialogueMgr();
            }
            return _instance;
        }
    }


    public DialogueMgr()
    {

    }

    private Dialogue cfgs = Dialogue.Data;

    private EndShow endCfgs = EndShow.Data;

    private int curDialogueId = -1;

    private int lastDialogueId = -1;

    private int _dialogueValue = 0;
    private int dialogueValue
    {
        get { return _dialogueValue; }
        set { _dialogueValue = value;
        }
    }

    private int startDialogueId = 1001;

    private const string DialoguePrefLastIdKey = "DialoguePrefLastIdKey";

    private const string DialoguePrefCurIdKey = "DialoguePrefCurIdKey";

    private const string DialoguePrefValueKey = "DialoguePrefValueKey";

    private const string IsGameKey = "DialogueIsGameValueKey";

    private const string GameTypeKey = "GameTypeKey";

    private const string GameIdKey = "GameIdKey";

    private const string GameDialogueIdKey = "GameDialogueIdKey";

    private bool IsGame = false;

    private int GameTypeA = -1;

    private int GameId = -1;

    private int GameDialogueId = 0;

    /// <summary>
    /// 获取对话配置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public DialogueItem GetDialogueCfgById(int id)
    {
        DialogueItem item = Instance.cfgs.Get(id);
        if (item == null)
        {
            Debug.LogError("没有找到对应的对话配置，id=" + id);
            return null;
        }
        return item;
    }

    /// <summary>
    ///  上句话是不是坐发言
    /// </summary>
    /// <returns></returns>
    public bool LastIsSpeekContent(DialogueSpeekType type)
    {
        if (lastDialogueId <= 0) return false;
        DialogueItem item = Instance.cfgs.Get(lastDialogueId);
        if (item == null)
        {
            return false;
        }
        bool isContent = item.ShowType == (int)DialogueType.Dialogue;
        if (!isContent) return false;

        return item.Speek == (int)type;
    }

    /// <summary>
    /// 执行选项
    /// </summary>
    public void OnSelectChoice(int jumpId,int addValue)
    {
        dialogueValue += addValue;
        CheckIsFinish(jumpId);
    }

    /// <summary>
    /// 重新开始剧情
    /// </summary>
    public void StartDialogue()
    {
        if(!UIMgr.IsUIOpen(UIConf.DialogueUI))
        {
            UIMgr.OpenUI(UIConf.DialogueUI);
        }
        curDialogueId = startDialogueId;
        lastDialogueId = -1;
        dialogueValue = 0;
        ResertPlayer();
    }

    private void ResertPlayer()
    {
        PlayerPrefs.SetInt(DialoguePrefCurIdKey, startDialogueId);
        PlayerPrefs.SetInt(DialoguePrefLastIdKey, -1);
        PlayerPrefs.SetInt(DialoguePrefValueKey, 0);
        PlayerPrefs.SetInt(IsGameKey, 0);
        PlayerPrefs.SetInt(GameTypeKey, 0);
        PlayerPrefs.SetInt(GameIdKey, 0);
        PlayerPrefs.SetInt(GameDialogueIdKey, 0);
        EventMgr.DispatchEvent(EventConf.RefreDialogueInfo, curDialogueId, lastDialogueId, false);
    }

    private void GetPlayer()
    {
        curDialogueId = PlayerPrefs.GetInt(DialoguePrefCurIdKey, startDialogueId);
        lastDialogueId = PlayerPrefs.GetInt(DialoguePrefLastIdKey, -1);
        dialogueValue = PlayerPrefs.GetInt(DialoguePrefValueKey, 0);
        IsGame = PlayerPrefs.GetInt(IsGameKey, 0) == 1;
        GameTypeA = PlayerPrefs.GetInt(GameTypeKey, 0);
        GameId = PlayerPrefs.GetInt(GameIdKey, 0);
        GameDialogueId = PlayerPrefs.GetInt(GameDialogueIdKey, 0);

    }

    private void SetPlayer()
    {
        PlayerPrefs.SetInt(IsGameKey, IsGame ? 1 : 0);
        PlayerPrefs.SetInt(GameTypeKey, GameTypeA);
        PlayerPrefs.SetInt(GameIdKey, GameId);

        PlayerPrefs.SetInt(DialoguePrefCurIdKey, curDialogueId);
        PlayerPrefs.SetInt(DialoguePrefLastIdKey, lastDialogueId);
        PlayerPrefs.SetInt(DialoguePrefValueKey, dialogueValue);
        PlayerPrefs.SetInt(GameDialogueIdKey, GameDialogueId);

    }

    /// <summary>
    /// 继续剧情
    /// </summary>
    public void NextDialogue()
    {
        if (!UIMgr.IsUIOpen(UIConf.DialogueUI))
        {
            UIMgr.OpenUI(UIConf.DialogueUI);
        }
        if(curDialogueId <=0)
        {
            GetPlayer();
        }
        if(IsGame)
        {
            GotoGame((GameType)GameTypeA, GameId, GameDialogueId);
            var cfg = GetDialogueCfgById(GameDialogueId);
            if (!string.IsNullOrEmpty(cfg.BGM))
            {
                SoundMgr.PlayBgm(cfg.BGM);
            }
            return;
        }

        EventMgr.DispatchEvent(EventConf.RefreDialogueInfo, curDialogueId, lastDialogueId,true);
    }

    public void CheckIsFinish(int nextId = 0)
    {
        DialogueItem cfg = GetDialogueCfgById(curDialogueId);
        if(cfg == null)
        {
            Debug.LogError("没有找到对应的对话配置，id=" + curDialogueId);
            return;
        }
        if(cfg.IsFinish == 1)
        {
            GotoFinish();
            ResertPlayer();
            return;
        }
        SetPlayer();
        TriggerNextDialogue(nextId);
    }

    public void TriggerNextDialogue(int nextId = 0)
    {
        if(nextId > 0)
        {
            lastDialogueId = curDialogueId;
            curDialogueId = nextId;
        }
        else
        {
            DialogueItem item = Instance.cfgs.Get(curDialogueId);
            if (item == null)
            {
                Debug.LogError("没有找到对应的对话配置，id=" + curDialogueId);
                return;
            }

            if(item.NextId.Length == 1)
            {
                lastDialogueId = curDialogueId;
                curDialogueId = item.NextId[0];
            }
            else if(item.NextId.Length == 0)
            {
                //UIMgr.CloseUI(UIConf.DialogueUI);
                //Debug.LogError("对话没有正确配置下一句对话:" + curDialogueId);
                EventMgr.DispatchEvent(EventConf.HideAllDialogue);
                return;
            }
            else
            {

                bool isOk = false;
                for (int i = 0; i < item.NextIdNeed.Length; i++)
                {
                    if (dialogueValue <= item.NextIdNeed[i])
                    {
                        lastDialogueId = curDialogueId;
                        curDialogueId = item.NextId[i];
                        isOk = true;
                        break;
                    }
                }
                if(!isOk)
                {
                    lastDialogueId = curDialogueId;
                    curDialogueId = item.NextId[item.NextId.Length - 1];
                }
            }
        }
        EventMgr.DispatchEvent(EventConf.RefreDialogueInfo, curDialogueId, lastDialogueId,false);
    }

    public void GotoFinish()
    {
        var list = Instance.endCfgs.items;
        if(list == null || list.Length<= 0)
        {
            Debug.LogError("结束配置没有内容");
            return;
        }

        for(int i = 0;i < list.Length;i++)
        {
            EndShowItem cfg = list[i];
            if(dialogueValue>= cfg.Value[0] && dialogueValue <= cfg.Value[1])
            {
                UIMgr.OpenUI(UIConf.EndUI, cfg);
                break;
            }
        }
    }
    public void GotoGame(GameType type,int gameId,int dialogueId)
    {
        switch(type)
        {
            case GameType.ChangeDialgue:
                //切换对话 will美好世界
                //goto
                UIMgr.OpenUI("LetterBoardUI", gameId);
                break;
        }
        IsGame = true;
        GameTypeA = (int)type;
        GameId = gameId;
        GameDialogueId = dialogueId;
        SetPlayer();
        UIMgr.Instance.SetTopLay(UIConf.DialogueUI);
        TriggerNextDialogue();
    }

    /// <summary>
    /// 结局索引，1开始，对应剧情表结束跳转配置 GameResult 字段
    /// </summary>
    /// <param name="resultIndex"></param>
    public void GameFinish(int resultIndex)
    {
        var cfg = GetDialogueCfgById(GameDialogueId);
        if(cfg == null)
        {
            Debug.LogError("没有找到对应的对话配置，id=" + GameDialogueId);
            return;
        }
       

        int nextId = -1;
        if(cfg.GameResult.Length == 1)
        {
            nextId = cfg.NextId[0];
        }
        else
        {
            if (resultIndex > cfg.GameResult.Length)
            {
                Debug.LogError("结局索引越界，id=" + GameDialogueId + " index=" + resultIndex);
                return;
            }
            nextId = cfg.GameResult[resultIndex - 1];

        }
        IsGame = false;
        GameTypeA = 0;
        GameId = 0;
        GameDialogueId = 0;
        if (nextId >= 0)
        {
            lastDialogueId = curDialogueId;
            curDialogueId = nextId;
            EventMgr.DispatchEvent(EventConf.RefreDialogueInfo, curDialogueId, lastDialogueId,false);
        }
    }
}

public enum DialogueType
{
    None = 0,
    Monologue = 1,//独白
    Dialogue = 2,//对话
    Seletion = 3,//选择
    Game=4,//小游戏
}

public enum DialogueSpeekType
{
    None = 0,
    Left = 1,
    Right= 2,
}

/// <summary>
/// 小游戏类型
/// </summary>
public enum GameType
{
    None = 0,
    ChangeDialgue = 1,
}