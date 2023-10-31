using System;
using UnityEngine;

namespace BenStudios.ScreenManagement
{
    public abstract class WindowBase : MonoBehaviour, IWindowBase
    {
        public abstract void EndAnimation(Action OnAnimationComplete);
        public abstract void StartAnimation();
        public abstract void OnCloseClick();
    }
}