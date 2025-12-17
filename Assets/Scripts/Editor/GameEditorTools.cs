using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public static class GameEditorTools
{
    [InitializeOnLoadMethod]
    private static void EditorInitializeMethod()
    {
        EditorApplication.playModeStateChanged += AutoSceneEnable;
        EditorApplication.update += LocationElement;
    }
    
    private static void LocationElement()
    {
        if (!Input.GetMouseButton(1)) return;

        var eventSystem = EventSystem.current;
        if (!eventSystem) return;

        var uiPointerEventData = new PointerEventData(eventSystem);
        uiPointerEventData.position = Input.mousePosition;

        var uiRayCastResultCache = new List<RaycastResult>();
        eventSystem.RaycastAll(uiPointerEventData, uiRayCastResultCache);
        if (uiRayCastResultCache.Count > 0)
        {
            Selection.activeObject = uiRayCastResultCache[0].gameObject;
        }
    }
    private static void AutoSceneEnable(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.ExitingEditMode)
        {
            var tmpScenes = new List<EditorBuildSettingsScene>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                scene.enabled = true;
                tmpScenes.Add(scene);
            }
            EditorBuildSettings.scenes = tmpScenes.ToArray();
        }

        EditorApplication.playModeStateChanged -= AutoSceneEnable;
    }

    public static bool DrawHeader(string headerTitle, string key, bool defaultState = true)
    {
        var state = EditorPrefs.GetBool(key, defaultState);
        if (state == false) GUI.backgroundColor = Color.white * 0.8f;
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);
        headerTitle = (state ? "\u25BC " : "\u25BA ") + "<b><size=12>" + headerTitle + "</size></b>";
        if (!GUILayout.Toggle(true, headerTitle, "dragTab", GUILayout.MinWidth(20f))) state = !state;
        if (GUI.changed) EditorPrefs.SetBool(key, state);
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = GUI.backgroundColor;
        GUILayout.Space(state ? 5f : 3f);
        return state;
    }
}