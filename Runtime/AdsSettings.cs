using UnityEngine;

namespace WhiteArrow.LevelPlayInitialization
{
    [CreateAssetMenu(menuName = "White Arrow/Ads Settings", fileName = "AdsSettings")]
    public class AdsSettings : ScriptableObject
    {
        [Header("Aps")]
#if UNITY_EDITOR || UNITY_ANDROID
        [SerializeField] private string _appKeyAndroid;
#endif

#if UNITY_EDITOR || UNITY_IOS
        [SerializeField] private string _appKeyIOS;
#endif

        [Header("Consent")]
        [SerializeField] private string _privacyPolicyUrl;
        [SerializeField] private PrivacyConsentConfirmer _privacyConsentConfirmer;
        [SerializeField] private bool _isFamilyDirected;
        [SerializeField] private bool _isChildeDirected;



        public string AppKey
        {
            get
            {
#if UNITY_EDITOR && UNITY_ANDROID || UNITY_ANDROID
                return _appKeyAndroid;
#elif UNITY_EDITOR && UNITY_IOS || UNITY_IOS
                return _appKeyIOS;
#else
                return string.Empty;
#endif
            }
        }


        public string PrivacyPolicyUrl => _privacyPolicyUrl;
        public PrivacyConsentConfirmer PrivacyConsentConfirmer => _privacyConsentConfirmer;
        public bool IsFamilyDirected => _isFamilyDirected;
        public bool IsChildeDirected => _isFamilyDirected ? true : _isChildeDirected;
    }
}
