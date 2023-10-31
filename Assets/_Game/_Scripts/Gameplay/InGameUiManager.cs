using BenStudios.ScreenManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUiManager : MonoBehaviour
{

    private void OnEnable()
    {
        GlobalEventHandler.OnBottomBoxIsFull += Callback_On_Bottom_Box_Full;
        GlobalEventHandler.OnAllBallsCleared += Callback_On_All_Balls_Cleared;
    }
    private void OnDisable()
    {
        GlobalEventHandler.OnBottomBoxIsFull -= Callback_On_Bottom_Box_Full;
        GlobalEventHandler.OnAllBallsCleared -= Callback_On_All_Balls_Cleared;
    }

    private void ShowLevelFailedPopup()
    {
        ScreenManager.Instance.ChangeScreen(Window.OutOfSpacePopup, ScreenType.Additive);
    }
    private void ShowLevelCompletePopup()
    {
        MyUtils.DelayedCallback(.5f, () =>
        {
            ScreenManager.Instance.ChangeScreen(Window.LevelCompletePopup, ScreenType.Additive);
        });
    }

    private void Callback_On_Bottom_Box_Full()
    {
        ShowLevelFailedPopup();
    }
    private void Callback_On_All_Balls_Cleared()
    {
        ShowLevelCompletePopup();
    }
}
