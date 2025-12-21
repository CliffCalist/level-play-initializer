using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_IOS && !CONSENT_ONLY_CHILDREN
using Unity.Advertisement.IosSupport;
#endif

namespace WhiteArrow
{
    public static class ConsentResolver
    {
        private static ConsentConfig s_config;


        private static IAgeOver18Confirmer s_ageGroupConfirmer;
        private static IConsentConfirmer s_consentConfirmer;



        private const string AGE_GROUP_CHOICE_SAVE_KEY = "age_group_choice";
        private const string CONSENT_CHOICE_SAVE_KEY = "consent_choice";



        public static bool IsDataProcessingAllowed => !IsFamilyDirected && !IsChildeDirected;
        public static bool IsFamilyDirected => GetConfig().IsFamilyDirected;
        public static bool IsChildeDirected => GetConfig().IsChildeDirected;


        public static bool IsResolved
        {
            get
            {
                if (IsDataProcessingAllowed)
                {
                    if (IsAgeRequireDefinition && !IsAgeBeenDefined())
                        return false;

                    if (IsConsentRequireDefinition && !IsConsentBeenDefined())
                        return false;
                }

                return true;
            }
        }

        public static bool IsAgeRequireDefinition
        {
            get
            {
                if (!IsDataProcessingAllowed)
                    return false;

                return !IsAgeBeenDefined();
            }
        }

        public static bool IsConsentRequireDefinition
        {
            get
            {
                if (!IsDataProcessingAllowed)
                    return false;

                return !IsConsentBeenDefined();
            }
        }



        #region Config
        public static void SetConfig(ConsentConfig config)
        {
            s_config = config ?? throw new ArgumentNullException(nameof(config));
        }

        private static ConsentConfig GetConfig()
        {
            ThrowIfConfigNotSet();
            return s_config;
        }

        private static void ThrowIfConfigNotSet()
        {
            if (s_config == null)
                throw new InvalidOperationException($"{nameof(ConsentResolver)} config is not set.");
        }
        #endregion



        #region Resolving
        public static async Task ResolveIfNeeded()
        {
            if (IsResolved)
                return;

            await ForceResolve();
        }

        public static async Task ForceResolve()
        {
            if (!IsDataProcessingAllowed)
                return;

            var ageConfirmer = GetAgeGroupConfirmer();
            var isAgeOver18 = await ageConfirmer.Confirm();
            SaveAgeOver18Choice(isAgeOver18);

            if (isAgeOver18)
            {
                var consentConfirmer = GetConsentConfirmer();
                var isConsentConfirmed = await consentConfirmer.Confirm();
                SaveConsentChoice(isConsentConfirmed);

                if (isAgeOver18 && isConsentConfirmed)
                    await RequestIDFAIfApplicableAsync();
            }
        }

        private static async Task RequestIDFAIfApplicableAsync()
        {
            if (!IsDataProcessingAllowed || !IsUserAgeOver18() || !IsConsentConfirmed())
            {
                Debug.Log("IDFA request skipped due to unmet conditions.");
                return;
            }

#if UNITY_IOS && !CONSENT_ONLY_CHILDREN
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
        #endregion



        #region  Age

        public static bool IsAgeBeenDefined()
        {
            return PlayerPrefs.HasKey(AGE_GROUP_CHOICE_SAVE_KEY);
        }

        public static bool IsUserAgeOver18()
        {
            if (!IsAgeBeenDefined())
                return false;

            var result = PlayerPrefs.GetString(AGE_GROUP_CHOICE_SAVE_KEY);
            return bool.TryParse(result, out var isOver18) ? isOver18 : false;
        }

        private static void SaveAgeOver18Choice(bool isOver18)
        {
            PlayerPrefs.SetString(AGE_GROUP_CHOICE_SAVE_KEY, isOver18.ToString());
        }

        private static IAgeOver18Confirmer GetAgeGroupConfirmer()
        {
            if (s_ageGroupConfirmer == null)
            {
                if (GetConfig().AgeGroupConfirmer is not Object ageGroupConfirmerPrefab)
                    throw new InvalidOperationException($"Age group confirmer prefab is not set or is not an UnityEngine.Object.");

                s_ageGroupConfirmer = (IAgeOver18Confirmer)Object.Instantiate(ageGroupConfirmerPrefab);
            }

            return s_ageGroupConfirmer;
        }
        #endregion



        #region Consent
        public static bool IsConsentBeenDefined()
        {
            return PlayerPrefs.HasKey(CONSENT_CHOICE_SAVE_KEY);
        }

        public static bool IsConsentConfirmed()
        {
            if (!IsConsentBeenDefined())
                return false;

            var result = PlayerPrefs.GetString(CONSENT_CHOICE_SAVE_KEY);
            return bool.TryParse(result, out var choice) ? choice : false;
        }

        private static void SaveConsentChoice(bool choice)
        {
            PlayerPrefs.SetString(CONSENT_CHOICE_SAVE_KEY, choice.ToString());
        }

        private static IConsentConfirmer GetConsentConfirmer()
        {
            if (s_consentConfirmer == null)
            {
                if (GetConfig().ConsentConfirmer is not Object consentConfirmerPrefab)
                    throw new InvalidOperationException($"Consent confirmer prefab is not set or is not an UnityEngine.Object.");

                s_consentConfirmer = (IConsentConfirmer)Object.Instantiate(consentConfirmerPrefab);
            }

            return s_consentConfirmer;
        }
        #endregion
    }
}