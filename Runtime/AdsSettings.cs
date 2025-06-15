using UnityEngine;

namespace WhiteArrow.AdFlow
{
    [CreateAssetMenu(menuName = "White Arrow/Ads Settings", fileName = "AdsSettings")]
    public class AdsSettings : ScriptableObject
    {
        [SerializeField] private string _appKeyAndroid;
        [SerializeField] private string _appKeyIOS;
        [SerializeField] private bool _isFamilyDirected;
        [SerializeField] private bool _isChildeDirected;

        [Space]
        [SerializeField] private Canvas _timerView;


        public string AppKey => Application.platform == RuntimePlatform.IPhonePlayer ? _appKeyIOS : _appKeyAndroid;
        public bool IsFamilyDirected => _isFamilyDirected;
        public bool IsChildeDirected => _isFamilyDirected ? true : _isChildeDirected;

        public Canvas TimerView => _timerView;
    }
}
