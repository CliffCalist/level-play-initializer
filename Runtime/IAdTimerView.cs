using System;

namespace WhiteArrow.AdFlow
{
    public interface IAdTimerView
    {
        void Activate(Action callback);
    }
}