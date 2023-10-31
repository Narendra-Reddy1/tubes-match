using BenStudios.ScreenManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteScreen : PopupBase
{
    public void OnClickNext()
    {
        ScreenManager.Instance.CloseLastAdditiveScreen(() =>
        {
            ScreenManager.Instance.RemoveAllScreens(() =>
            {
                ScreenManager.Instance.ChangeScreen(Window.GameplayScreen, isUIObject: false);
            });
        });
    }
}
