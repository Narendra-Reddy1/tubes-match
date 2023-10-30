using System;

public static class GlobalEventHandler
{
    public static Action<BallEntity> OnBallEntitySelected = default;

    public static Action OnBottomBoxIsFull = default;
}
