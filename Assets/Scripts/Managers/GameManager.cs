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

        private void Awake()
        {
            HideGateway();
            _puzzleManager.DisplayFinishMessage += DisplayFinishGateway;
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
