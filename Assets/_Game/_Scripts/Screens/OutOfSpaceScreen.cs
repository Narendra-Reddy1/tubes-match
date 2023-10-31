using BenStudios.ScreenManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfSpaceScreen : PopupBase
{

    public void OnClickTryAgain()
    {
        ScreenManager.Instance.CloseLastAdditiveScreen(() =>
        {
            ScreenManager.Instance.RemoveAllScreens(() =>
            {
                ScreenManager.Instance.ChangeScreen(Window.GameplayScreen, isUIObject: false);
            });
        });
    }

    public void OnClickPlayOn()
    {

    }
}
