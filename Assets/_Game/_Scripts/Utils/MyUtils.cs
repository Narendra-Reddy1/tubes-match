using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MyUtils
{

    public static void Log(object message, LogType logType = LogType.Log)
    {
        switch (logType)
        {
            case LogType.Log:
                Debug.Log($"{message}");
                break;
            case LogType.Warning:
                Debug.LogWarning($"{message}");
                break;
            case LogType.Error:
                Debug.LogError($"{message}");
                break;
            case LogType.Assert:
                Debug.LogAssertion($"{message}");
                break;
            default:
                Debug.Log($"{logType}: {message}");
                break;
        }
    }
    public static bool IsApplicationConnectedToInternet()
    {
        return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;


    }
    public static string ObjectToJson<T>(T data)
    {
        return JsonUtility.ToJson(data);
    }
    public static T JsonToObject<T>(string key)
    {
        return JsonUtility.FromJson<T>(key);
    }
    public static Vector3 GetMousePositionInWorldCordinates(Vector2 mousePosition, Camera camera)
    {
        if (camera == null) camera = Camera.main;
        return camera.ScreenToWorldPoint(mousePosition);
    }
    public static void LoadSceneAsync(int sceneIndex, bool makeActive = false, System.Action onComplete = null)
    {
        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive).completed += (handle) =>
        {
            if (makeActive)
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(sceneIndex));
            onComplete?.Invoke();
        };
    }
    public static void LoadSceneAsync(string sceneName, bool makeActive = false, System.Action onComplete = null)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).completed += (handle) =>
        {
            if (makeActive)
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            onComplete?.Invoke();
        };
    }
    public static void UnloadSceneAsync(string sceneName, System.Action onComplete = null)
    {
        SceneManager.UnloadSceneAsync(sceneName).completed += (handle) =>
        {
            onComplete?.Invoke();
        };
    }
    public static void UnloadSceneAsync(int sceneIndex, System.Action onComplete = null)
    {
        SceneManager.UnloadSceneAsync(sceneIndex).completed += (handle) =>
        {
            onComplete?.Invoke();
        };
    }
    public static string GetFormattedSeconds(int seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

        string formattedString = "";
        if (timeSpan.Days > 0)
            formattedString = timeSpan.ToString(@"d\d\ h\h");
        else if (timeSpan.Hours > 0)
            formattedString = timeSpan.ToString(@"h\h\ m\m");
        else
            formattedString = timeSpan.ToString(@"mm\:ss");
        return formattedString;
    }

    /// <summary>
    /// For executing all Coroutines in which EXACTLY one argument needs to be passed, call this method
    /// Here the delay is always fixed, hence we pass WaitForSeconds object and do not create it dynamically
    /// </summary>
    public static void DelayedCallback<type>(double delay, Action<type> callBack, type t)
    {
        _Delayed_Callback(delay, callBack, t).Forget();
    }
    /// <summary>
    /// For executing all Coroutines in which no parameters need to be passed, call this method
    /// </summary>
    public static void DelayedCallback(float delay, Action callBack)
    {
        _Delayed_Callback(delay, callBack).Forget();
    }

    /// <summary>
    /// For executing all Coroutines in which EXACTLY one argument needs to be passed, call this method
    /// </summary>
    public static void DelayedCallback<type>(float delay, Action<type> callBack, type t)
    {
        _Delayed_Callback(delay, callBack, t).Forget();
    }

    /// <summary>
    /// For executing all Coroutines in which EXACTLY TWO argument needs to be passed, call this method
    /// </summary>
    public static void DelayedCallback<type>(float delay, Action<type, type> callBack, type t1, type t2)
    {
        _Delayed_Callback(delay, callBack, t1, t2).Forget();

    }


    private static async UniTaskVoid _Delayed_Callback(double delayInSeconds, Action callBack)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delayInSeconds), ignoreTimeScale: false);
        callBack();
    }
    private static async UniTaskVoid _Delayed_Callback<type>(float delay, Action<type, type> callBack, type t1, type t2)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false);
        callBack(t1, t2);
    }
    private static async UniTaskVoid _Delayed_Callback<type>(double delayInSeconds, Action<type> callBack, type t)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delayInSeconds), ignoreTimeScale: false);
        callBack(t);
    }

}
