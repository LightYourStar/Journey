using Game.Utility;
using UnityEngine;

namespace Game
{
    public class MonoKeyBinds : MonoBehaviour
    {
        SceneSwitching sceneSwitching;

        private void Awake()
        {
            Cursor.visible = false; //hide the cursor

            sceneSwitching = FindObjectOfType<SceneSwitching>();
        }

        private void Update()
        {
            Util.RunIfKeyDown(KeyCode.R, sceneSwitching.ReloadScene);
            Util.RunIfKeyDown(KeyCode.N, sceneSwitching.NextScene);
        }
    }
}