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


        private const string SETTINGS_RESOURCE_PATH = "AdsSettings";



        public static async Task InitializeAsync()
        {
            s_settings = Resources.Load<AdsSettings>(SETTINGS_RESOURCE_PATH);
            if (s_settings == null)
            {
                Debug.LogError("[AdFlow] AdsSettings not found in Resources.");
                return;
            }

            await RequestConsentAsync();
            await RequestIDFAAsync();
            await InitializeLevelPlay();
        }

        private static async Task RequestConsentAsync()
        {
            await Task.CompletedTask;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private static async Task RequestIDFAAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
#endif
        }

        private static async Task InitializeLevelPlay()
        {
            ApplyPrivacyMetaData();

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

            LevelPlay.Init(appKey);
            await tcs.Task;
        }

        private static void ApplyPrivacyMetaData()
        {
            var gdprValue = s_settings.Consent;
            LevelPlay.SetConsent(gdprValue);

            var ccpaValue = (!gdprValue).ToString().ToLower();
            LevelPlay.SetMetaData("do_not_sell", ccpaValue);

            var isFamilyDirected = s_settings.IsFamilyDirected.ToString();
            LevelPlay.SetMetaData("Google_Family_Self_Certified_SDKS", isFamilyDirected);

            var isChildeDirected = s_settings.IsChildeDirected.ToString();
            LevelPlay.SetMetaData("is_deviceid_optout", isChildeDirected);
            LevelPlay.SetMetaData("is_child_directed", isChildeDirected);
            LevelPlay.SetMetaData("UnityAds_coppa", isChildeDirected);
        }
    }
}