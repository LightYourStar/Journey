using System;
using JO.UIManager;
using System.Collections.Generic;
namespace UIManager
{
    public static class UIConf
    {
        private static Dictionary<string,UILayer> uilayerMap = new Dictionary<string, UILayer>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, Type> scriptTypeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        public static void InitRegisterScriptType()
        {
            RegisterScriptType(DialogueUI, typeof(DialogueUI));
            RegisterUILayer(DialogueUI, UILayer.FullScreen);
            RegisterScriptType(EndUI, typeof(EndUI));
            RegisterUILayer(EndUI, UILayer.FullScreen);
            RegisterScriptType(GameUI, typeof(GameUI));
            RegisterUILayer(GameUI, UILayer.FullScreen);
            RegisterScriptType(LetterBoardUI, typeof(LetterBoardUI));
            RegisterUILayer(LetterBoardUI, UILayer.Pop);
            RegisterScriptType(LoadingUI, typeof(LoadingUI));
            RegisterUILayer(LoadingUI, UILayer.Top);
            RegisterScriptType(StartUI, typeof(StartUI));
            RegisterUILayer(StartUI, UILayer.FullScreen);
            RegisterScriptType(TextGameUI, typeof(TextGameUI));
            RegisterUILayer(TextGameUI, UILayer.FullScreen);
        }
        public static Type GetScriptType(string name)
        {
            if (scriptTypeMap.TryGetValue(name, out Type type))
            {
                return type;
            }
            return null;
        }
        public static UILayer GetUILayer(string name)
        {
            if (uilayerMap.TryGetValue(name, out UILayer layer))
            {
                return layer;
            }
            return UILayer.FullScreen;
        }
        private static void RegisterScriptType(string name, Type type)
        {
            if (type != null && !scriptTypeMap.ContainsKey(name))
            {
                scriptTypeMap.Add(name, type);
            }
        }
        private static void RegisterUILayer(string name, UILayer layer)
        {
            uilayerMap.Add(name, layer);
        }
        public const string DialogueUI = "DialogueUI";
        public const string EndUI = "EndUI";
        public const string GameUI = "GameUI";
        public const string LetterBoardUI = "LetterBoardUI";
        public const string LoadingUI = "LoadingUI";
        public const string StartUI = "StartUI";
        public const string TextGameUI = "TextGameUI";
    }
}
