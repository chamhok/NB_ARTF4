using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    #region Fields
    private Dictionary<string, UnityEngine.Object> _resources = new();

    #endregion

    #region Properties

    public bool LoadBase { get; set; }
    public bool LoadIntro { get; set; }
    public bool LoadGame { get; set; }
    public bool LoadLoading { get; set; }

    #endregion

    // 리소스 비동기 로드 메서드
    #region Asynchronous Loading

    /// <summary>
    /// 콜백을 처리하는 제네릭 핸들러입니다.
    /// </summary>
    private void AsyncHandlerCallback<T>(string key, AsyncOperationHandle<T> handle, Action<T> callback) where T : UnityEngine.Object
    {
        handle.Completed += operationHandle =>
        {
            _resources.Add(key, operationHandle.Result);
            callback?.Invoke(operationHandle.Result);
        };
    }

    /// <summary>
    /// 비동기 방식으로 리소스를 로드하고 콜백을 호출합니다.
    /// </summary>
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        string loadKey = key;
        if (_resources.TryGetValue(loadKey, out UnityEngine.Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        if (loadKey.Contains(".sprite"))
        {
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(loadKey);
            AsyncHandlerCallback(loadKey, handle, callback as Action<Sprite>);
        }
        else
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(loadKey);
            AsyncHandlerCallback(loadKey, handle, callback);
        }
    }

    /// <summary>
    /// 특정 라벨에 속한 모든 리소스를 비동기 방식으로 로드하고 콜백을 호출합니다.
    /// </summary>
    public void AllLoadAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        var operation = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        operation.Completed += operationHandle =>
        {
            int loadCount = 0;
            int totalCount = operationHandle.Result.Count;
            foreach (var result in operationHandle.Result)
            {
                LoadAsync<T>(result.PrimaryKey, obj =>
                {
                    loadCount++;
                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                });
            }
        };
    }
    /// <summary>
    /// 특정 라벨에 속한 모든 리소스를 비동기 방식으로 언로드합니다.
    /// </summary>
    public void UnloadAllAsync(string label)
    {
        var operation = Addressables.LoadResourceLocationsAsync(label);
        operation.Completed += operationHandle =>
        {
            foreach (var result in operationHandle.Result)
            {
                if (!_resources.TryGetValue(result.PrimaryKey, out UnityEngine.Object resource)) continue;
                Addressables.Release(resource);
                _resources.Remove(result.PrimaryKey);
                Debug.Log($"{resource} 언로드");
            }
        };
    }
    #endregion

    // 리소스 동기 로드&언로드 메서드
    #region Synchronous Loading

    /// <summary>
    /// 리소스를 동기적으로 로드합니다.
    /// </summary>
    public T Load<T>(string key) where T : UnityEngine.Object
    {
        if (_resources.TryGetValue(key, out UnityEngine.Object resource)) return resource as T;
        Debug.LogError($"키를 찾을 수 없습니다. : {key}");
        return null;
    }

    #endregion

    // 프리펩 인스턴스화 메서드
    #region Instantiation

    /// <summary>
    /// 프리팹을 인스턴스화하고 생성된 인스턴스를 반환합니다.
    /// </summary>
    public GameObject InstantiatePrefab(string key, Transform transform = null)
    {
        GameObject resource = Load<GameObject>(key);

        GameObject instance = Instantiate(resource, transform);

        if (instance != null) return instance;
        Debug.LogError($"리소스를 인스턴스화하지 못했습니다.: { key}");
        return null;
    }

    #endregion
}

