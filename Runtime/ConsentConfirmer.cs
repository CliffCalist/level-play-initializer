using System;
using UnityEngine;

namespace WhiteArrow
{
    public abstract class ConsentConfirmer : MonoBehaviour
    {
        internal protected abstract void Confirm(Action<bool> onCompleted);
    }
}