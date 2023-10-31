using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableAssetLoader : AddressableAssetLocationsLoader
{
    public static AddressableAssetLoader Instance;
    private Action OnDownloadComplete;
    [SerializeField] private bool cleaCacheOnGameRestart = false;
    [SerializeField]
    private List<string> assetlables =
           new List<string>() {
            //"Scene",
            //"Prefab",
                   //"Texture",
                   //"Material",
                   // "Model"
                   "preload"
           };
    private ConcurrentDictionary<string, AsyncOperationHandle<SceneInstance>> sceneDictionAry = new ConcurrentDictionary<string, AsyncOperationHandle<SceneInstance>>();
    private void Awake()
    {
        Instance = this;

    }

    #region Public Methods
    public IEnumerator InitAdressableAsset(Action<int, int, string, float> _OnAssetDownloading, Action _OnDownloadComplete)
    {
        OnDownloadComplete = _OnDownloadComplete;
        yield return InitLocations(cleaCacheOnGameRestart, assetlables, _OnAssetDownloading);

        if (IsDownloading && IsDownloadingCompleted)
        {
            yield return new WaitForSeconds(2);
        }

        OnDownloadComplete?.Invoke();
    }
    public AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
    {
        if (GetAssetLocation(sceneName, out IResourceLocation resourceLocation1))
        {

            var result = Addressables.LoadSceneAsync(resourceLocation1, loadSceneMode);
            sceneDictionAry.AddOrUpdate(sceneName, result, (key, oldValue) => oldValue);
            return result;
        }
        else
        {
            Debug.Log("AdressableAssetLoader,LoadScene : " + "Resource Location Not Exist For this Scnename");
            return default;
        }
    }


    public AsyncOperationHandle<SceneInstance> ReleaseScene(string sceneName)
    {
        if (sceneDictionAry.TryGetValue(sceneName, out AsyncOperationHandle<SceneInstance> asyncOperationHandle))
        {
            var result = Addressables.UnloadSceneAsync(asyncOperationHandle);
            sceneDictionAry.TryRemove(sceneName, out AsyncOperationHandle<SceneInstance> asyncOperationHandle1);
            return result;
        }
        else
        {
            Debug.Log("AdressableAssetLoader,ReleaseScene : " + "Unbale unload scene");
            return default;
        }
    }

    public async void LoadAssetOld(string name, Action<UnityEngine.Object> OnCompleted)
    {
        if (GetAssetLocation(name, out IResourceLocation resourceLocation1))
        {
            var async = Addressables.LoadAssetAsync<UnityEngine.Object>(resourceLocation1);
            await async.Task;
            if (async.Status == AsyncOperationStatus.Succeeded)
            {
                OnCompleted.Invoke(async.Result);
            }
            else
            {
                OnCompleted.Invoke(null);
            }
        }
        else
        {
            OnCompleted.Invoke(null);
        }
    }
    public async void LoadAssetAsync<T>(string name, Action<bool, AsyncOperationHandle<T>> OnCompleted) where T : class
    {

        var async = Addressables.LoadAssetAsync<T>(name);
        await async.Task;
        if (async.Status == AsyncOperationStatus.Succeeded)
        {
            OnCompleted.Invoke(true, async);
        }
        else
        {
            OnCompleted.Invoke(false, default);
        }

    }

    public async void LoadAsset(string name, Action<UnityEngine.Object> OnCompleted)
    {

        var async = Addressables.LoadAssetAsync<UnityEngine.Object>(name);
        await async.Task;
        if (async.Status == AsyncOperationStatus.Succeeded)
        {
            OnCompleted.Invoke(async.Result);
        }
        else
        {
            OnCompleted.Invoke(null);
        }

    }
    public async void Instantiate(string name, Transform parent, bool instantiateInWorldSpace, Action<GameObject> OnCompleted)
    {
        if (GetAssetLocation(name, out IResourceLocation resourceLocation1))
        {
            var async = Addressables.InstantiateAsync(resourceLocation1, parent, instantiateInWorldSpace);
            await async.Task;
            if (async.Status == AsyncOperationStatus.Succeeded)
            {
                OnCompleted.Invoke(async.Result);
            }
            else
            {
                OnCompleted.Invoke(null);
            }
        }
        else
        {
            OnCompleted.Invoke(null);
        }
    }
    public void ReleaseInstance(string name, GameObject obj)
    {
        if (GetAssetLocation(name, out IResourceLocation resourceLocation1))
        {
            if (obj != null)
            {
                Addressables.ReleaseInstance(obj);
            }
        }
    }
    public void Release<TObj>(AsyncOperationHandle<TObj> asyncObj) where TObj : UnityEngine.Object
    {
        if (asyncObj.IsValid())
        {
            Addressables.Release(asyncObj);
        }
    }
    public async void Instantiate(AssetReferenceGameObject obj, Transform parent, bool instantiateInWorldSpace, Action<bool, AsyncOperationHandle<GameObject>> OnCompleted)
    {

        if (obj.RuntimeKeyIsValid())
        {
            AsyncOperationHandle<GameObject> async = Addressables.InstantiateAsync(obj, parent, instantiateInWorldSpace);
            await async.Task;
            // Debug.Log("Instantiate  valid key");
            if (async.Status == AsyncOperationStatus.Succeeded)
            {
                OnCompleted?.Invoke(true, async);
                return;
            }
        }
        else
        {
            Debug.Log("Instantiate not valid key");
        }
        OnCompleted.Invoke(false, default);

    }
    public async void LoadTexture(AssetReferenceTexture obj, Action<bool, AsyncOperationHandle<Texture>> OnCompleted)
    {

        if (obj.RuntimeKeyIsValid())
        {
            var async = obj.LoadAssetAsync();
            await async.Task;
            // Debug.Log("Instantiate  valid key");
            if (async.Status == AsyncOperationStatus.Succeeded)
            {
                OnCompleted.Invoke(true, async);
                return;
            }
        }
        else
        {
            Debug.Log("Loading texture Key not valid key");
        }
        OnCompleted.Invoke(false, default);

    }
    public async void LoadSprite(AssetReferenceSprite obj, Action<bool, AsyncOperationHandle<Sprite>> OnCompleted)
    {

        if (obj.RuntimeKeyIsValid())
        {
            var async = obj.LoadAssetAsync();
            await async.Task;
            // Debug.Log("Instantiate  valid key");
            if (async.Status == AsyncOperationStatus.Succeeded)
            {
                OnCompleted.Invoke(true, async);
                return;
            }
        }
        else
        {
            Debug.Log("Loading texture Key not valid key");
        }
        OnCompleted.Invoke(false, default);

    }
    public void Release(GameObject asyncObj)
    {
        if (asyncObj != null)
        {
            Addressables.Release(asyncObj);
        }
    }
    public void Release(string name, UnityEngine.Object obj)
    {
        if (GetAssetLocation(name, out IResourceLocation resourceLocation1))
        {
            if (obj != null)
            {
                Addressables.Release(obj);
            }
        }
    }
    #endregion

    #region Private Methods
    private bool isAdressLocationsLoaded;
    public ref bool IsAdressLocationsLoaded()
    {
        return ref isAdressLocationsLoaded;
    }
    public void SetIsAdressLocationsLoaded()
    {
        isAdressLocationsLoaded = true;
    }
    public bool GetAssetLocation(string assetName, out IResourceLocation resourceLocation)
    {
        return accetLocation.TryGetValue(assetName, out resourceLocation);
    }
    //private IEnumerator LoadAssetData(IResourceLocation resourceLocation, Action<UnityEngine.Object> OnCompleted)
    //{

    //    var async = Addressables.LoadAssetAsync<UnityEngine.Object>(resourceLocation);
    //    yield return async;
    //    if (async.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        OnCompleted.Invoke(async.Result);
    //    }
    //    else
    //    {
    //        OnCompleted.Invoke(null);
    //    }
    //}
    //private IEnumerator InstantiateObject(IResourceLocation resourceLocation,Transform parent, bool instantiateInWorldSpace, Action<GameObject> OnCompleted)
    //{

    //    var async = Addressables.InstantiateAsync(resourceLocation, parent, instantiateInWorldSpace);
    //    yield return async;
    //    if (async.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        OnCompleted.Invoke(async.Result);
    //    }
    //    else
    //    {
    //        OnCompleted.Invoke(null);
    //    }
    //}
    #endregion

}

