using TMPro;
using UnityEngine;

namespace SlidingPuzzle
{
    public class TimeManager : MonoBehaviour
    {
        private readonly int MAX_SECOND = 59;
        private readonly int SECOND = 1;

        [SerializeField] TextMeshProUGUI _counter;
        [SerializeField] int _minute = 2;
        [SerializeField] int _second = 0;

        private bool _isTimeOut = false;

        private void Start()
        {
            _counter.text = _minute.ToString() + " : " + _second.ToString();
        }

        public void CountDown()
        {
            _second--;

            if (_second < 0)
            {
                _minute--;

                if(_minute < 0)
                {
                    _isTimeOut = true;
                    StopCountDown();
                }

                _second = MAX_SECOND;
            }

            _counter.text = _minute.ToString() + " : " + _second.ToString();
            Invoke(nameof(CountDown), SECOND);
        }

        public void StopCountDown()
        {
            CancelInvoke(nameof(CountDown));
        }
        
        public bool IsTimeOut
        { 
            get { return _isTimeOut; } 
        }
    }
}
