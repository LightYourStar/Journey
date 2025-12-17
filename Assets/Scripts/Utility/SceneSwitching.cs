using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Utility
{
    public class SceneSwitching : MonoBehaviour
    {
        private const int SceneCount = 5;
        private int sceneIndex = 0; //the index of the currently active scene


        //called when the script is being loaded
        private void Awake()
        {
            if (FindObjectsOfType<SceneSwitching>().Length > 1) //if there are more than one objects of this type, destroy self
            {
                Destroy(gameObject); //destroy self
                return;//early return
            }

            //add this gameObject to don't destroy on load
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Sets the active scene to the next and reloads.
        /// </summary>
        public void NextScene()
        {
            sceneIndex++; //increase the scene index by one

            if (sceneIndex >= SceneCount) //if the sceneIndex doesn't exceed the amount of scenes built
            {
                Application.Quit(); //quit the application
                return; //early return
            }

            //reload the scene
            ReloadScene(); //increase the index
        }

        /// <summary>
        /// Reloads the current active scene
        /// </summary>
        public void ReloadScene()
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}