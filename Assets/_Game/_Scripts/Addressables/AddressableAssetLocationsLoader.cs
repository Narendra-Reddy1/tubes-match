using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
public class PreloadAddresableAssetInit
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void OnBeforeSplashScreenLoadRuntimeMethod()
    {
        var asyncOperation = Addressables.InitializeAsync();
        asyncOperation.Completed += OnCompleted;
    }
    private static void OnCompleted(AsyncOperationHandle<IResourceLocator> obj)
    {
       //GlobalVariables.isAdressableInitlized = true;
    }
}
public class AddressableAssetLocationsLoader : MonoBehaviour
{
    private Action<int, int, string, float> OnAssetDownloading;
    protected ConcurrentDictionary<string, IResourceLocation> accetLocation = new ConcurrentDictionary<string, IResourceLocation>();
    protected bool IsDownloading = false;
    protected bool IsDownloadingCompleted = false;
    private int totalNeedToDownload;
    private int alreadyDownloaded;
    private List<string> numberOfLabelneedsToDownload = new List<string>();
    private IList<IResourceLocation> preLoadLocations = new List<IResourceLocation>();

    public bool pIsDownloadingCompleted { get => IsDownloadingCompleted; }

    protected IEnumerator InitLocations(bool cleaCacheOnGameRestart, List<string> assetlables, Action<int, int, string, float> _OnAssetDownloading)
    {
        OnAssetDownloading = _OnAssetDownloading;
        yield return Addressables.InitializeAsync();
        yield return UpdateCatalogCoro();
        yield return GetTotalNumberOfFilesNeedToDownload(cleaCacheOnGameRestart, assetlables);
        foreach (var label in numberOfLabelneedsToDownload)
        {

            IsDownloading = true;
            yield return DownloadLabelAsset(label);

        }
        yield return LoadLocation(assetlables);
        Debug.LogError("IsDownloadingCompleted");
        IsDownloadingCompleted = true;
    }
    protected IEnumerator DownloadLabelAsset(string label)
    {
        var downloadAsync = Addressables.DownloadDependenciesAsync(label, false);
        while (!downloadAsync.IsDone)
        {
            float percent = downloadAsync.PercentComplete;
            OnAssetDownloading?.Invoke(totalNeedToDownload, alreadyDownloaded, label, downloadAsync.PercentComplete * 100);
            yield return new WaitForEndOfFrame();
        }
        alreadyDownloaded++;
        OnAssetDownloading?.Invoke(totalNeedToDownload, alreadyDownloaded, label, 100);
        Addressables.Release(downloadAsync);
        Debug.Log($"{label} UpdateAssets finish");
    }
    protected IEnumerator UpdateCatalogCoro()
    {
        List<string> catalogsToUpdate = new List<string>();
        var checkCatalogHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkCatalogHandle;
        if (checkCatalogHandle.Status == AsyncOperationStatus.Succeeded)
            catalogsToUpdate = checkCatalogHandle.Result;
        Addressables.Release(checkCatalogHandle);
        if (catalogsToUpdate.Count > 0)
        {
            var updateCatalogHandle = Addressables.UpdateCatalogs(catalogsToUpdate, false);
            yield return updateCatalogHandle;
            Addressables.Release(updateCatalogHandle);
        }
    }
    private IEnumerator LoadLocation(List<string> assetlables)
    {
        foreach (var label in assetlables)
        {
            AsyncOperationHandle<IList<IResourceLocation>> locationsAsync = Addressables.LoadResourceLocationsAsync(label);
            yield return locationsAsync;
            if (locationsAsync.Status == AsyncOperationStatus.Succeeded)
            {
                IList<IResourceLocation> locations = locationsAsync.Result;
                if (locations.Count > 0)
                {
                    foreach (IResourceLocation resourceLocation in locations)
                    {
                        //Debug.LogError("LoadLocation ResourceLocation: " + resourceLocation.InternalId + " " + resourceLocation.ProviderId);//Debug only
                        preLoadLocations.Add(resourceLocation);
                        accetLocation.AddOrUpdate(resourceLocation.PrimaryKey, resourceLocation, (key, oldValue) => oldValue);
                    }
                }
            }
        }
    }

    private IEnumerator GetTotalNumberOfFilesNeedToDownload(bool cleaCacheOnGameRestart, List<string> assetlables)
    {
        totalNeedToDownload = 0;
        numberOfLabelneedsToDownload.Clear();
        foreach (var label in assetlables)
        {
            if (cleaCacheOnGameRestart)
            {
                Addressables.ClearDependencyCacheAsync(label);
            }
            var async = Addressables.GetDownloadSizeAsync(label);
            yield return async;
            long updateLabelSize = 0;
            if (async.Status == AsyncOperationStatus.Succeeded)
                updateLabelSize = async.Result;
            Addressables.Release(async);
            if (updateLabelSize > 0)
            {
                numberOfLabelneedsToDownload.Add(label);
                totalNeedToDownload++;
                Debug.Log($"{label} last version");
            }
        }
    }
    public IEnumerator LoadAllAssetsByKey()
    {
        //Will load all objects that match the given key.  
        //If this key is an Addressable label, it will load all assets marked with that label
        //AsyncOperationHandle<IList<UnityEngine.Object>> loadWithSingleKeyHandle = Addressables.LoadAssetsAsync<UnityEngine.Object>("preload", obj =>
        //{
        //    //Gets called for every loaded asset
        //    Debug.Log(obj.name);
        //    UniTask.DelayFrame(5);
        //});
        //yield return loadWithSingleKeyHandle;
        AsyncOperationHandle<UnityEngine.Object> opHandle;
        for (int i = 0; i < preLoadLocations.Count; i++)
        {
            opHandle = Addressables.LoadAssetAsync<UnityEngine.Object>(preLoadLocations[i]);
            yield return opHandle;
            //Debug.LogError("Loaded: " + opHandle.DebugName);
            //yield return new WaitForEndOfFrame();
        }

        Debug.LogError("LoadAllAssetsByKey done");
    }
}
public enum DownloadStatus
{
    Downloading=0,
    DownloadCompletd=1,
    DownloadSuccesfull=2,
}
