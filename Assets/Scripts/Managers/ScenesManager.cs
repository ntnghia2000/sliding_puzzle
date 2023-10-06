using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlidingPuzzle
{
    public class ScenesManager: MonoBehaviour
    {
        public static ScenesManager Instance;

        private List<string> _scenes = new List<string>();

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }    

            GetListScenes();
        }

        private void GetListScenes()
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                    _scenes.Add(sceneName);
                }   
            }
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene(_scenes[0]);
        }

        public void LoadNextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void LoadSceneByIndex(int scene)
        {
            SceneManager.LoadScene(_scenes[scene]);
        }
        
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }    
    }
}
