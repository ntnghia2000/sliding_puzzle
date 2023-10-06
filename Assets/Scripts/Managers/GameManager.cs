using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SlidingPuzzle
{
    public delegate void DisplayFinishMessageCallback(bool isWin);

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject _finishGateway;
        [SerializeField] private GameObject _winMessage;
        [SerializeField] private GameObject _loseMessage;
        [SerializeField] private PuzzleManager _puzzleManager;
        [SerializeField] private Button _loseReplay;
        [SerializeField] private Button _loseBack;
        [SerializeField] private Button _winNext;
        [SerializeField] private Button _winBack;

        private void Awake()
        {
            HideGateway();
            CreateButtonListener();
            _puzzleManager.DisplayFinishMessage += DisplayFinishGateway;
        }

        private void CreateButtonListener()
        {
            _loseBack.onClick.AddListener(BackToMainMenu);
            _loseReplay.onClick.AddListener(ReloadScene);
            _winNext.onClick.AddListener(LoadNextScene);
            _winBack.onClick.AddListener(BackToMainMenu);
        }

        private void BackToMainMenu()
        {
            ScenesManager.Instance.BackToMainMenu();
        }
        
        private void ReloadScene()
        {
            ScenesManager.Instance.ReloadScene();
        }
        
        private void LoadNextScene()
        {
            if(SceneManager.GetActiveScene().buildIndex != 2)
            {
                ScenesManager.Instance.LoadSceneByIndex(2);
            }
            else
            {
                ScenesManager.Instance.BackToMainMenu();
            }    
        }    

        private void HideGateway()
        {
            _finishGateway.SetActive(false);
            _winMessage.SetActive(false);
            _loseMessage.SetActive(false);
        }    

        private void DisplayFinishGateway(bool isWin)
        {
            if(isWin)
            {
                DisplayWinMessage();
            }
            else
            {
                DisplayLoseMessage();
            }

            _finishGateway.SetActive(true);
        }
        
        private void DisplayWinMessage()
        {
            _winMessage.SetActive(true);
        }

        private void DisplayLoseMessage()
        {
            _loseMessage.SetActive(true);
        }
    }
}
