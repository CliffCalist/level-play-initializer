using System;
using System.Threading.Tasks;
using GoogleMobileAds.Ump.Api;
using Unity.Services.LevelPlay;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace WhiteArrow.AdFlow
{
    public static class AdFlow
    {
        private static AdsSettings s_settings;
        private static RewardedAdProvider s_rewardedAd = new();
        private static IAdTimerView s_timerView;



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
            var isRequestCompleted = false;
            var request = new ConsentRequestParameters();

            ConsentInformation.Update(request, error =>
            {
                if (error != null)
                {
                    Debug.LogWarning($"[AdFlow] Consent update failed: {error.Message}");
                    isRequestCompleted = true;
                    return;
                }

                if (ConsentInformation.IsConsentFormAvailable())
                {
                    ConsentForm.LoadAndShowConsentFormIfRequired(error =>
                    {
                        if (error != null)
                        {
                            Debug.LogWarning($"[AdFlow] Consent form load failed: {error.Message}");
                            isRequestCompleted = true;
                            return;
                        }

                        if (ConsentInformation.CanRequestAds())
                            Debug.Log("[AdFlow] Consent granted.");
                        else Debug.Log("[AdFlow] Consent denied.");

                        isRequestCompleted = true;
                    });
                }
                else isRequestCompleted = true;
            });

            while (!isRequestCompleted)
                await Task.Yield();
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
            var gdprValue = ConsentInformation.CanRequestAds();
            IronSource.Agent.setConsent(gdprValue);

            var ccpaValue = (!gdprValue).ToString().ToLower();
            IronSource.Agent.setMetaData("do_not_sell", ccpaValue);

            var isFamilyDirected = s_settings.IsFamilyDirected.ToString();
            IronSource.Agent.setMetaData("Google_Family_Self_Certified_SDKS", isFamilyDirected);

            var isChildeDirected = s_settings.IsChildeDirected.ToString();
            IronSource.Agent.setMetaData("is_deviceid_optout", isChildeDirected);
            IronSource.Agent.setMetaData("is_child_directed", isChildeDirected);
            IronSource.Agent.setMetaData("UnityAds_coppa", isChildeDirected);
        }



        public static void ShowRewardedAdWithTimer(Action<bool> callback)
        {
            if (!RewardedAdProvider.IsAvailable)
            {
                Debug.LogWarning("[AdFlow] Rewarded video is not available.");
                callback?.Invoke(false);
                return;
            }

            var view = GetOrCreateTimerInstance();

            if (view == null)
            {
                Debug.LogWarning($"[AdFlow] {nameof(s_settings.TimerView)} not assigned. Skipping timer.");
                s_rewardedAd.Show(callback);
            }
            else view.Activate(() => s_rewardedAd.Show(callback));
        }

        private static IAdTimerView GetOrCreateTimerInstance()
        {
            if (s_timerView == null)
            {
                var canvas = Object.Instantiate(s_settings.TimerView);

                s_timerView = canvas.GetComponentInChildren<IAdTimerView>(true);
                if (s_timerView == null)
                {
                    Debug.LogWarning($"[AdFlow] {nameof(s_settings.TimerView)} is assigned, but it doesn't have a {nameof(IAdTimerView)} component.");
                    Object.Destroy(canvas.gameObject);
                }
                else Object.DontDestroyOnLoad(canvas);
            }

            return s_timerView;
        }
    }
}