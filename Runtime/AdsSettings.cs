using System.Collections.Generic;
using com.unity3d.mediation;
using UnityEngine;

namespace WhiteArrow.LevelPlayInitialization
{
    [CreateAssetMenu(menuName = "White Arrow/Ads Settings", fileName = "AdsSettings")]
    public class AdsSettings : ScriptableObject
    {
        [Header("Consent")]
        [SerializeField] private string _privacyPolicyUrl;
        [SerializeField] private PrivacyConsentConfirmer _privacyConsentConfirmer;
        [SerializeField] private bool _isFamilyDirected;
        [SerializeField] private bool _isChildeDirected;


        [Header("Settings")]
#if UNITY_EDITOR || UNITY_ANDROID
        [SerializeField] private string _appKeyAndroid;
#endif

#if UNITY_EDITOR || UNITY_IOS
        [SerializeField] private string _appKeyIOS;
#endif


        [Header("For legacy API support")]
#pragma warning disable CS0618 // Type or member is obsolete
        [SerializeField] private List<LevelPlayAdFormat> _adFormats;
#pragma warning restore CS0618 // Type or member is obsolete



        public string PrivacyPolicyUrl => _privacyPolicyUrl;
        public PrivacyConsentConfirmer PrivacyConsentConfirmer => _privacyConsentConfirmer;
        public bool IsFamilyDirected => _isFamilyDirected;
        public bool IsChildeDirected => _isFamilyDirected ? true : _isChildeDirected;


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


#pragma warning disable CS0618 // Type or member is obsolete
        public IReadOnlyList<LevelPlayAdFormat> AdFormats => _adFormats;
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
