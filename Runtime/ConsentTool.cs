using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace WhiteArrow
{
    public static class ConsentTool
    {
        private static ConsentConfirmer s_confirmerPrefab;
        private static ConsentConfirmer s_confirmerInstance;

        public static bool IsIDFAEnabled = true;

        private static readonly List<IConsentChoiceListener> s_listeners = new();



        private const string CONSENT_CHOICE_SAVE_KEY = "consent_choice";



        public static void AddListener(IConsentChoiceListener listener)
        {
            s_listeners.Add(listener);
            listener.OnConsentChoiceChanged(IsConsentConfirmed());
        }

        public static bool RemoveListener(IConsentChoiceListener listener)
        {
            return s_listeners.Remove(listener);
        }

        private static void NotifyListeners()
        {
            var isConsentConfirmed = IsConsentConfirmed();

            foreach (var listener in s_listeners)
                listener.OnConsentChoiceChanged(isConsentConfirmed);
        }



        public static void SetConfirmerPrefab(ConsentConfirmer confirmer)
        {
            if (s_confirmerPrefab != null)
                throw new InvalidOperationException("Confirmer prefab is already set.");

            s_confirmerPrefab = confirmer ?? throw new ArgumentNullException(nameof(confirmer));
        }

        private static ConsentConfirmer GetConfirmerInstance()
        {
            if (s_confirmerInstance == null)
            {
                s_confirmerInstance = Object.Instantiate(s_confirmerPrefab);
                Object.DontDestroyOnLoad(s_confirmerInstance.gameObject);
            }

            return s_confirmerInstance;
        }



        public static async Task RequestConsentIfNeeded()
        {
            if (IsConsentBeenRequested())
                return;

            await RequestConsent();
        }

        public static async Task RequestConsent()
        {
            var confirmer = GetConfirmerInstance();
            var isConformed = await confirmer.Confirm();

            SaveConsentChoice(isConformed);
            NotifyListeners();

            if (isConformed)
                await RequestIDFAAsync();
        }

        private static async Task RequestIDFAAsync()
        {
            if (!IsIDFAEnabled)
            {
                Debug.LogWarning("IDFA is disabled.");
                return;
            }

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
            Debug.Log("IDFA is not supported on this platform.");
            await Task.CompletedTask;
#endif
        }



        public static bool IsConsentBeenRequested()
        {
            return PlayerPrefs.HasKey(CONSENT_CHOICE_SAVE_KEY);
        }

        public static bool IsConsentConfirmed()
        {
            if (!IsConsentBeenRequested())
                return false;

            var result = PlayerPrefs.GetString(CONSENT_CHOICE_SAVE_KEY);
            return result == true.ToString();
        }

        private static void SaveConsentChoice(bool choice)
        {
            PlayerPrefs.SetString(CONSENT_CHOICE_SAVE_KEY, choice.ToString());
        }
    }
}