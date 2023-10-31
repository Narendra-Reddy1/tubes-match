using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BenStudios.ScreenManagement
{
    [System.Serializable]
    public enum Window
    {
        None,
        LevelCompletePopup,
        OutOfSpacePopup,
        GameplayScreen,
    }

    [System.Serializable]
    public class WindowObject
    {
        [SerializeField]
        private GameObject gameObjectRef;
        private GameObject gameObject;

        #region Public Methods
        public ref GameObject GetObj()
        {
            return ref gameObject;
        }
        public GameObject Instantiate(Transform parent, bool instantiateInWorldSpace)
        {
            gameObject = GameObject.Instantiate(gameObjectRef, parent, instantiateInWorldSpace);
            return gameObject;
        }
        public void Release()
        {
            GameObject.Destroy(gameObject);
        }
        #endregion
    }

    [System.Serializable]
    public class WindowAddressableObject
    {
        [SerializeField]
        private AssetReferenceGameObject assetRefObj;
        private AsyncOperationHandle<GameObject> asyncObj;
        [HideInInspector]
        public GameObject gameObject;
        [HideInInspector]
        public bool IsObjectExist = false;
        private bool IsObjectinstantiating = false;
        #region Public Methods
        public ref GameObject GetObj()
        {
            return ref gameObject;
        }
        public void Instantiate(Transform parent, bool instantiateInWorldSpace, System.Action<bool> OnComplete)
        {
            if (IsObjectinstantiating)
            {
                OnComplete.Invoke(false);
            }
            IsObjectinstantiating = true;
            if (IsObjectExist)
            {
                OnComplete.Invoke(true);
                return;
            }


            Debug.Log("asset ref obj " + assetRefObj);
            Debug.Log("parent " + parent);
            Debug.Log("instantiateInWorldSpace " + instantiateInWorldSpace);
            Debug.Log("AddressableAssetLoader.Instance. " + AddressableAssetLoader.Instance);

            AddressableAssetLoader.Instance.Instantiate(assetRefObj, parent, instantiateInWorldSpace,
                (status, _asynobj) =>
                {
                    IsObjectinstantiating = false;
                    if (status)
                    {
                        asyncObj = _asynobj;
                        gameObject = asyncObj.Result;
                        IsObjectExist = true;
                        OnComplete.Invoke(true);
                    }
                    else
                    {
                        OnComplete.Invoke(false);
                        IsObjectExist = false;
                    }
                });
        }
        public void Release()
        {
            IsObjectinstantiating = false;
            if (IsObjectExist)
            {
                AddressableAssetLoader.Instance.Release(asyncObj);
                // assetRefObj.ReleaseAsset();

            }
            IsObjectExist = false;
            gameObject = null;
        }
        #endregion
    }
}