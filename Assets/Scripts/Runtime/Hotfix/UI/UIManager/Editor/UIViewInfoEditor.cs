using JO.UIManager;
using System.Collections.Generic;
using UIManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UIViewInfo))]
public class UIViewInfoEditor : Editor
{
    private Vector2 scrollPosition;
    private const string DragAndDropID = "UIComComponentDragArea";

    public override void OnInspectorGUI()
    {
        UIViewInfo info = target as UIViewInfo;

        if (info == null) return;

        if (info.uiCom == null)
        {
            info.uiCom = new List<GameObject>();
        }

        EditorGUILayout.Space(10);

        info.UILayer = (UILayer)EditorGUILayout.EnumPopup("UI界面层级", info.UILayer);

        EditorGUILayout.Space();

        if (info.UILayer == UILayer.Top || info.UILayer == UILayer.Tip)
        {
            info.isExclusion = EditorGUILayout.Toggle("是否是互斥界面", info.isExclusion);
            EditorGUILayout.Space(10);
        }

        EditorGUILayout.Space(10);


        if (GUILayout.Button("自动补全Panel信息"))
        {
            info.AutoInfo();

           
        }

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.LabelField($"UI组件列表 (数量: {info.uiCom.Count})", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

        for (int i = 0; i < info.uiCom.Count; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.window);
            EditorGUILayout.BeginHorizontal();

            GameObject comp = info.uiCom[i];
            if (comp != null)
            {
                EditorGUILayout.ObjectField(comp, typeof(Component), false);
            }
            else
            {
                EditorGUILayout.LabelField($"元素 {i} (空引用)", EditorStyles.miniLabel);
            }


            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        EditorGUILayout.EndScrollView();


        EditorGUILayout.EndVertical();
    }

}