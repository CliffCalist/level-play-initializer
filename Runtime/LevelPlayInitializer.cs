using System.Linq;
using System.Threading.Tasks;
using Unity.Services.LevelPlay;
using UnityEngine;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace WhiteArrow.LevelPlayInitialization
{
    public static class LevelPlayInitializer
    {
        private static AdsSettings s_settings;
        private static PrivacyConsentConfirmer s_privacyConsentConfirmer;


        private const string SETTINGS_RESOURCE_PATH = "AdsSettings";
        private const string PRIVACY_CONSENT_CHOICE_SAVE_KEY = "PrivacyConsent";



        public static async Task InitializeAsync(AdsSettings settings = null)
        {
            if (settings == null)
                s_settings = Resources.Load<AdsSettings>(SETTINGS_RESOURCE_PATH);
            else s_settings = settings;

            if (s_settings == null)
            {
                Debug.LogError("[AdFlow] AdsSettings not found in Resources.");
                return;
            }

            if (!IsPrivacyConsentBeenRequested())
                await RequestPrivacyConsentAsync();

            await RequestIDFAAsync();
            await InitializeLevelPlay();
        }



        private static async Task RequestPrivacyConsentAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            var confirmer = GetPrivacyConsentConfirmer();
            confirmer.Confirm(s_settings.PrivacyPolicyUrl, result => tcs.TrySetResult(result));

            var result = await tcs.Task;
            SavePrivacyConsent(result);
        }

        public static void RequestPrivacyConsent()
        {
            var confirmer = GetPrivacyConsentConfirmer();
            confirmer.Confirm(s_settings.PrivacyPolicyUrl, SavePrivacyConsent);
        }

        private static PrivacyConsentConfirmer GetPrivacyConsentConfirmer()
        {
            if (s_privacyConsentConfirmer == null)
            {
                s_privacyConsentConfirmer = Object.Instantiate(s_settings.PrivacyConsentConfirmer);
                Object.DontDestroyOnLoad(s_privacyConsentConfirmer.gameObject);
            }

            return s_privacyConsentConfirmer;
        }



        private static void SavePrivacyConsent(bool consent)
        {
            PlayerPrefs.SetString(PRIVACY_CONSENT_CHOICE_SAVE_KEY, consent.ToString());
            Debug.Log($"[AdFlow] Privacy consent: {consent}");
        }

        private static bool GetSavedPrivacyConsent()
        {
            if (!IsPrivacyConsentBeenRequested())
                return false;

            var result = PlayerPrefs.GetString(PRIVACY_CONSENT_CHOICE_SAVE_KEY);
            return result == true.ToString();
        }

        private static bool IsPrivacyConsentBeenRequested()
        {
            return PlayerPrefs.HasKey(PRIVACY_CONSENT_CHOICE_SAVE_KEY);
        }



        private static async Task RequestIDFAAsync()
        {
#if UNITY_IOS
            if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking();

                while (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                       ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    await Task.Yield();
                }
            }
#else
            Debug.Log("[AdFlow] IDFA is not supported on this platform.");
            await Task.CompletedTask;
#endif
        }



        private static async Task InitializeLevelPlay()
        {
            UpdateAllMetaData();

            var tcs = new TaskCompletionSource<bool>();
            var appKey = s_settings.AppKey;

            LevelPlay.OnInitSuccess += _ =>
            {
                Debug.Log($"[AdFlow] LevelPlay initialized with app key: {appKey}");
                tcs.TrySetResult(true);
            };

            LevelPlay.OnInitFailed += error =>
            {
                Debug.LogError($"[AdFlow] LevelPlay initialization failed: {error}");
                tcs.TrySetResult(false);
            };

            LevelPlay.Init(appKey, adFormats: s_settings.AdFormats.ToArray());
            await tcs.Task;
        }



        private static void UpdateAllMetaData()
        {
            UpdatePrivacyConsentMetaData();
            UpdateFamilyMetaData();
            UpdateChildMetaData();
        }

        private static void UpdatePrivacyConsentMetaData()
        {
            var gdprValue = GetSavedPrivacyConsent();
            LevelPlay.SetConsent(gdprValue);

            var ccpaValue = (!gdprValue).ToString().ToLower();
            LevelPlay.SetMetaData("do_not_sell", ccpaValue);
        }

        private static void UpdateFamilyMetaData()
        {
            var isFamilyDirected = s_settings.IsFamilyDirected.ToString();
            LevelPlay.SetMetaData("Google_Family_Self_Certified_SDKS", isFamilyDirected);
        }

        private static void UpdateChildMetaData()
        {
            var isChildeDirected = s_settings.IsChildeDirected.ToString();
            LevelPlay.SetMetaData("is_deviceid_optout", isChildeDirected);
            LevelPlay.SetMetaData("is_child_directed", isChildeDirected);
            LevelPlay.SetMetaData("UnityAds_coppa", isChildeDirected);
        }
    }
}