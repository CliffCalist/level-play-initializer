using System;
using UnityEngine;

namespace WhiteArrow.AdFlow
{
    internal class RewardedAdProvider
    {
        private bool _isBusy;
        private Action<bool> _callback;
        private bool _rewarded;



        public static bool IsAvailable => IronSource.Agent.isRewardedVideoAvailable();



        public void Show(Action<bool> onCompleted)
        {
            if (!IsAvailable)
            {
                onCompleted?.Invoke(false);
                return;
            }

            if (_isBusy)
            {
                Debug.LogWarning("[AdFlow] Rewarded video is already being shown.");
                onCompleted?.Invoke(false);
                return;
            }

            _isBusy = true;
            _callback = onCompleted;
            _rewarded = false;
            SubscribeEvents();

            IronSource.Agent.showRewardedVideo();
        }



        private void SubscribeEvents()
        {
            IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewarded;
            IronSourceRewardedVideoEvents.onAdClosedEvent += OnClosed;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnFailed;
        }

        private void UnsubscribeEvents()
        {
            IronSourceRewardedVideoEvents.onAdRewardedEvent -= OnRewarded;
            IronSourceRewardedVideoEvents.onAdClosedEvent -= OnClosed;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent -= OnFailed;
        }



        private void OnRewarded(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            _rewarded = true;
        }

        private void OnClosed(IronSourceAdInfo adInfo)
        {
            _callback?.Invoke(_rewarded);
            Cleanup();
        }

        private void OnFailed(IronSourceError error, IronSourceAdInfo adInfo)
        {
            _callback?.Invoke(false);
            Cleanup();
        }



        private void Cleanup()
        {
            _callback = null;
            _rewarded = false;
            UnsubscribeEvents();
            _isBusy = false;
        }
    }
}