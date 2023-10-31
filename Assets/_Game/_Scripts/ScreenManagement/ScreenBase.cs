using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenStudios.ScreenManagement
{
    public class ScreenBase : WindowBase
    {
        #region PublicMethods
        public override void EndAnimation(Action OnAnimationComplete)
        {
            //Do nothing
        }

        public override void OnCloseClick()
        {
        }

        public override void StartAnimation()
        {
            //Do nothing
        }
        #endregion PublicMethods
    }

}