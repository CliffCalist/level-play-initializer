using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace WhiteArrow.AdFlow
{
    public class DefaultAdTimerView : MonoBehaviour, IAdTimerView
    {
        [SerializeField, Min(0)] private int _timeInSeconds = 3;
        [SerializeField] private TMP_Text _timerText;


        private bool _isActive;
        private Action _onDeactivated;



        public void Activate(Action onDeactivated)
        {
            if (_isActive)
                throw new InvalidOperationException("Timer is already active.");

            _isActive = true;
            _onDeactivated = () =>
            {
                _isActive = false;
                onDeactivated?.Invoke();
            };

            gameObject.SetActive(true);
            StartCoroutine(LaunchTimer());
        }

        private IEnumerator LaunchTimer()
        {
            var waitOneSecond = new WaitForSecondsRealtime(1);

            for (var i = _timeInSeconds; i >= 0; i--)
            {
                _timerText.text = i.ToString();
                yield return waitOneSecond;
            }

            _onDeactivated?.Invoke();
            gameObject.SetActive(false);
        }
    }
}