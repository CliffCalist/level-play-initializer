using System;
using UnityEngine;

namespace WhiteArrow.LevelPlayInitialization
{
    public abstract class PrivacyConsentConfirmer : MonoBehaviour
    {
        internal protected abstract void Confirm(string privacyPolicyUrl, Action<bool> onCompleted);
    }
}