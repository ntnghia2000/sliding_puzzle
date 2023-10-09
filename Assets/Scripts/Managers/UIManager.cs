using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlidingPuzzle
{
    public class UIManager : MonoBehaviour
    {
        private readonly int ADDITION_SCENE_INDEX = 1;

        [SerializeField] private int _maxSceneIndex = 2;

        public void LoadSceneByIndex(int index)
        {
            ScenesManager.Instance.LoadSceneByIndex(index);
        }

        public void BackToMainMenu()
        {
            ScenesManager.Instance.BackToMainMenu();
        }

        public void ReloadScene()
        {
            ScenesManager.Instance.ReloadScene();
        }

        public void LoadNextScene()
        {
            if (SceneManager.GetActiveScene().buildIndex + ADDITION_SCENE_INDEX > _maxSceneIndex)
            {
                ScenesManager.Instance.LoadSceneByIndex(0);
            }
            else
            {
                ScenesManager.Instance.LoadSceneByIndex(SceneManager.GetActiveScene().buildIndex + ADDITION_SCENE_INDEX);
            }
        }
    }
}
