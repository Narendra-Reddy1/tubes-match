using System;

namespace BenStudios
{
    public interface IWindowBase
    {
        public void StartAnimation();
        public void EndAnimation(Action OnAnimationComplete);
        public void OnCloseClick();
    }
}