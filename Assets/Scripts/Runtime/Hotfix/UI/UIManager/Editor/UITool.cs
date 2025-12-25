using JO.UIManager;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UIManager
{
    public class UITool
    {
        private static string UIViewPath = Application.dataPath + "/Resources/Prefabs/UIView";
        private static string UIConfPath = Application.dataPath +  "/Scripts/UI/UIManager/UIConf.cs";
        private static string GenPath = Application.dataPath + "/Scripts/UI/Gen";
        private static string LogicPath = Application.dataPath + "/Scripts/UI/Logic";

        private static HashSet<Type> supportedTypes = new HashSet<Type>
            {
                typeof(Transform),
                typeof(RectTransform),
                typeof(UnityEngine.UI.Text),
                typeof(UnityEngine.UI.Button),
                typeof(UnityEngine.UI.Toggle),
                typeof(UnityEngine.UI.Slider),
                typeof(UnityEngine.UI.Scrollbar),
                typeof(UnityEngine.UI.InputField),
                typeof(UnityEngine.UI.Image),
                typeof(UnityEngine.UI.RawImage),
                typeof(Animator)
            };

        [MenuItem("UITool/自动生成UIConf")]
        static void AutoUIConfig()
        {
            
            AutoUIConf();
            AutoUIComGen();
        }
        
        private static void AutoUIConf()
        {
            string[] files = System.IO.Directory.GetFiles(UIViewPath, "*.prefab", System.IO.SearchOption.AllDirectories);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using JO.UIManager;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("namespace UIManager");
            sb.AppendLine("{");
            sb.AppendLine("    public static class UIConf");
            sb.AppendLine("    {");
            sb.AppendLine("        private static Dictionary<string,UILayer> uilayerMap = new Dictionary<string, UILayer>(StringComparer.OrdinalIgnoreCase);");
            sb.AppendLine("        private static Dictionary<string, Type> scriptTypeMap = new Dictionary<string, " +
                          "Type>(StringComparer.OrdinalIgnoreCase);");

            sb.AppendLine("        public static void InitRegisterScriptType()");
            sb.AppendLine("        {");
            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                sb.AppendLine($"            RegisterScriptType({fileName}, typeof({fileName}));");
                
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(file.Replace(Application.dataPath, "Assets"));
                var uiViewInfo = gameObject.GetComponent<UIViewInfo>();
                if (uiViewInfo != null)
                {
                    sb.AppendLine($"            RegisterUILayer({fileName}, UILayer.{uiViewInfo.UILayer});");
                }

            }
            sb.AppendLine("        }");

            sb.AppendLine("        public static Type GetScriptType(string name)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (scriptTypeMap.TryGetValue(name, out Type type))");
            sb.AppendLine("            {");
            sb.AppendLine("                return type;");
            sb.AppendLine("            }");
            sb.AppendLine("            return null;");
            sb.AppendLine("        }");

            sb.AppendLine("        public static UILayer GetUILayer(string name)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (uilayerMap.TryGetValue(name, out UILayer layer))");
            sb.AppendLine("            {");
            sb.AppendLine("                return layer;");
            sb.AppendLine("            }");
            sb.AppendLine("            return UILayer.FullScreen;");
            sb.AppendLine("        }");


            sb.AppendLine("        private static void RegisterScriptType(string name, Type type)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (type != null && !scriptTypeMap.ContainsKey(name))");
            sb.AppendLine("            {");
            sb.AppendLine("                scriptTypeMap.Add(name, type);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");


            sb.AppendLine("        private static void RegisterUILayer(string name, UILayer layer)");
            sb.AppendLine("        {");
            sb.AppendLine(
                "            uilayerMap.Add(name, layer);");
            sb.AppendLine(
                "        }");

            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                sb.AppendLine($"        public const string {fileName} = \"{fileName}\";");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            System.IO.File.WriteAllText(UIConfPath, sb.ToString());
            AssetDatabase.Refresh();
            Debug.Log("UIConf.cs生成完成");


        }

        private static void AutoUIComGen()
        {
            string[] files = System.IO.Directory.GetFiles(UIViewPath, "*.prefab", System.IO.SearchOption.AllDirectories);
            foreach (string file in files)
            {
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(file.Replace(Application.dataPath, "Assets"));
                if (gameObject != null)
                {
                    UIViewInfo info = gameObject.GetComponent<UIViewInfo>();
                    if (info == null)
                    {
                        info = gameObject.AddComponent<UIViewInfo>();
                        AssetDatabase.SaveAssets();
                    }
                    if (info != null)
                    {
                        info.AutoInfo();
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.AppendLine("using JO.UIManager;");
                        sb.AppendLine("using UnityEngine;");
                        sb.AppendLine("using UnityEngine.UI;");
                        sb.AppendLine("namespace UIManager");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tpublic partial class {fileName}:UIViewBase");
                        sb.AppendLine("\t{");
                        sb.AppendLine(
                            "\t\t//自动生成的UI组件代码，请勿手动修改，如需修改请修改对应的Prefab并重新生成");
                     
                        int index = 0;
                        foreach (GameObject go in info.uiCom)
                        {
                            Component[] components = go.GetComponents<Component>();
                            foreach (Component comp in components)
                            {
                                if (comp == null) continue;
                                Type type = comp.GetType();
                                if (UITool.supportedTypes.Contains(type))
                                {
                                    
                                    sb.AppendLine(
                                        $"\t\tpublic {type.Name} {go.name}_{type.Name}");
                                    sb.AppendLine("\t\t{");
                                    sb.AppendLine($"\t\t\tget");
                                    sb.AppendLine("\t\t\t{");
                                    sb.AppendLine(
                                        $"\t\t\t\treturn UIViewInfo.uiCom[{index}].GetComponent<{type.Name}>();");
                                    sb.AppendLine("\t\t\t}");

                                    sb.AppendLine("\t\t}");
                                    
                                }
                            }
                            index++;
                        }

                        sb.AppendLine("\t}");

                        sb.AppendLine("}");

                        string genFilePath = System.IO.Path.Combine(GenPath, fileName + ".cs");
                        System.IO.File.WriteAllText(genFilePath, sb.ToString());
                        AssetDatabase.Refresh();
                        Debug.Log($"{fileName}.cs生成完成");

                        //生成逻辑脚本
                       
                        if (!System.IO.Directory.Exists(System.IO.Path.Combine(LogicPath, fileName)))
                        {
                            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(LogicPath, fileName));
                        }
                        string logicFilePath = System.IO.Path.Combine(LogicPath, fileName+"/"+fileName + ".cs");
                        if (!System.IO.File.Exists(logicFilePath))
                        {
                            System.Text.StringBuilder logicSb = new System.Text.StringBuilder();
                            logicSb.AppendLine("using System;");
                            logicSb.AppendLine("using JO.UIManager;");
                            logicSb.AppendLine("using UnityEngine;");
                            logicSb.AppendLine("namespace UIManager");
                            logicSb.AppendLine("{");
                            logicSb.AppendLine($"\tpublic partial class {fileName}");
                            logicSb.AppendLine("\t{");


                            logicSb.AppendLine("\t\tpublic override void OnInit()");
                            logicSb.AppendLine("\t\t{");
                            logicSb.AppendLine("\t\t}");
                            logicSb.AppendLine();
                            logicSb.AppendLine("\t\tpublic override void OnOpen(object param1 = null, object param2 = null, object param3 = null)");
                            logicSb.AppendLine("\t\t{");
                            logicSb.AppendLine("\t\t}");
                            logicSb.AppendLine();
                            logicSb.AppendLine("\t\tpublic override void OnClose()");
                            logicSb.AppendLine("\t\t{");
                            logicSb.AppendLine("\t\t}");
                            logicSb.AppendLine();
                            logicSb.AppendLine("\t\tpublic override void OnUpdate(float dt)");
                            logicSb.AppendLine("\t\t{");
                            logicSb.AppendLine("\t\t}");


                            logicSb.AppendLine("\t}");
                            logicSb.AppendLine("}");
                            System.IO.File.WriteAllText(logicFilePath, logicSb.ToString());
                            AssetDatabase.Refresh();
                            Debug.Log($"{fileName}.cs逻辑脚本生成完成");
                        }
                    }
                }
            }
        }
    }
}