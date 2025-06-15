using System;

namespace WhiteArrow.AdFlow
{
    internal static class RewardedAdService
    {
        private static Action<bool> s_callback;
        private static bool s_rewarded;



        public static bool IsAvailable => IronSource.Agent.isRewardedVideoAvailable();



        public static void Show(Action<bool> onCompleted)
        {
            if (!IsAvailable)
            {
                onCompleted?.Invoke(false);
                return;
            }

            s_callback = onCompleted;
            s_rewarded = false;
            SubscribeEvents();

            IronSource.Agent.showRewardedVideo();
        }



        private static void SubscribeEvents()
        {
            IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewarded;
            IronSourceRewardedVideoEvents.onAdClosedEvent += OnClosed;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnFailed;
        }

        private static void UnsubscribeEvents()
        {
            IronSourceRewardedVideoEvents.onAdRewardedEvent -= OnRewarded;
            IronSourceRewardedVideoEvents.onAdClosedEvent -= OnClosed;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent -= OnFailed;
        }



        private static void OnRewarded(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            s_rewarded = true;
        }

        private static void OnClosed(IronSourceAdInfo adInfo)
        {
            s_callback?.Invoke(s_rewarded);
            Cleanup();
        }

        private static void OnFailed(IronSourceError error, IronSourceAdInfo adInfo)
        {
            s_callback?.Invoke(false);
            Cleanup();
        }



        private static void Cleanup()
        {
            s_callback = null;
            s_rewarded = false;
            UnsubscribeEvents();
        }
    }
}