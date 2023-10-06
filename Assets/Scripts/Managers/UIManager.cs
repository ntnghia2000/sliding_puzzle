using UnityEngine;

namespace SlidingPuzzle
{
    public class UIManager : MonoBehaviour
    {
        public void LoadSceneByIndex(int index)
        {
            ScenesManager.Instance.LoadSceneByIndex(index);
        }    
    }
}
