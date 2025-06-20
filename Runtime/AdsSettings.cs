using UnityEngine;

namespace WhiteArrow.LevelPlayInitialization
{
    [CreateAssetMenu(menuName = "White Arrow/Ads Settings", fileName = "AdsSettings")]
    public class AdsSettings : ScriptableObject
    {
        [Header("Aps")]
        [SerializeField] private string _appKeyAndroid;
        [SerializeField] private string _appKeyIOS;

        [Header("Consent")]
        [SerializeField] private bool _isFamilyDirected;
        [SerializeField] private bool _isChildeDirected;



        public string AppKey => Application.platform == RuntimePlatform.IPhonePlayer ? _appKeyIOS : _appKeyAndroid;

        public bool IsFamilyDirected => _isFamilyDirected;
        public bool IsChildeDirected => _isFamilyDirected ? true : _isChildeDirected;
    }
}
