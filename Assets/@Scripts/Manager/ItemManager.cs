using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#region Item Class

#endregion

#region ItemManager Class
/// <summary>
/// 아이템을 관리하는 매니저 클래스입니다.
/// </summary>
public class ItemManager : MonoBehaviour
{
    #region Field
    /// <summary>
    /// 아이템의 목록을 저장하는 딕셔너리입니다.
    /// </summary>
    private Dictionary<int, Item> _hadItems = new Dictionary<int, Item>();
    // 필드 아이템 데이터를 저장하는 딕셔너리입니다.
    public Dictionary<int, Item> FieldItems = new Dictionary<int, Item>();

    // 아이템 프리팹을 저장하는 딕셔너리입니다.
    private Dictionary<int, GameObject> _itemPrefabs = new Dictionary<int, GameObject>();

    // 데이터 매니저 인스턴스입니다.
    private DataManager _dataManager;

    #endregion

    public void Initialized()
    {
        _dataManager = Main.DataManager;
        // JSON 파일에서 아이템 데이터를 로드합니다.
        LoadItemDataFromJson();

        // 아이템 프리팹을 로드합니다.
        LoadItemPrefabs();
    }
   
    #region Item management methods


    /// <summary>
    /// 아이템을 추가하는 메서드입니다.
    /// </summary>
    public void AddItem(Item item)
    {
        if (_hadItems.ContainsKey(item.Id))
        {
            // 기존 아이템의 활성 상태를 업데이트합니다.
            _hadItems[item.Id].Duration += _hadItems[item.Id].Duration;
        }
        else
        {
            foreach (var hadItem in _hadItems)
            {
                if (hadItem.Value.IsActivate)
                {
                    RemoveItem(hadItem.Value);
                }
            }
            _hadItems.Add(item.Id, item);
            _hadItems[item.Id].IsActivate = true;
        }

        Main.PlayerControl.StartActivateItem(item);
    }

    
    /// <summary>
    /// 아이템을 제거하는 메서드입니다.
    /// </summary>
    public void RemoveItem(Item item)
    {
        if (_hadItems.ContainsKey(item.Id))
        {
            item.IsActivate = false;
            _hadItems.Remove(item.Id);
            Main.PlayerControl.RemoveItemsEffectFromPlayer(item);
        }
        else
        {
            System.Console.WriteLine($"아이템을 찾을 수 없습니다: {item.Id}");
        }
    }


    #endregion


    // JSON 파일에서 아이템 데이터를 로드하는 메서드입니다.
    private void LoadItemDataFromJson()
    {
        // JSON 파일을 로드하여 아이템 데이터 컨테이너를 가져옵니다.
        ItemDataContainer itemDataContainer = _dataManager.itemLoader();

        // 아이템 데이터 컨테이너에서 아이템 데이터를 가져와 필드 아이템 딕셔너리에 저장합니다.
        foreach (var data in itemDataContainer.Items)
        {
            var itemData = data.Value;
            Item item = new();

            Debug.Log(data.Value.name);
            item.Initialize(data.Key, itemData.name, itemData.category, itemData.description, itemData.power, itemData.duration);
            FieldItems.TryAdd(item.Id, item);
        }
    }

    // 아이템 프리팹을 로드하는 메서드입니다.
    private void LoadItemPrefabs()
    {
        // 필드 아이템 딕셔너리에서 아이템 데이터를 가져와 아이템 프리팹을 로드하고 아이템 프리팹 딕셔너리에 저장합니다.
        foreach (var item in FieldItems)
        {
            string keyName = item.Key.ToString();
            // 아이템 프리팹을 로드합니다.
            GameObject itemPrefab = Main.Resource.Load<GameObject>($"{keyName}.prefab");

            // 아이템 프리팹 딕셔너리에 아이템 프리팹을 저장합니다.
            _itemPrefabs.TryAdd(item.Key, itemPrefab);
        }
    }

    // 아이템 스포너에서 아이템을 인스턴스화하는 메서드입니다.
    public void InstantiateItemsFromSpawner()
    {
        // 아이템 스포너를 로드합니다.
        GameObject spawner = Main.Resource.InstantiatePrefab("ItemPos.prefab");


        /*   // 아이템 스포너에서 스폰 포인트를 가져옵니다.
           ItemSpawnInfo[] spawnPoints = spawner.GetComponentsInChildren<ItemSpawnInfo>();*/

        // 각 스폰 포인트에서 아이템 ID를 가져와 해당 ID를 가진 아이템 프리팹을 인스턴스화합니다.
        foreach (Transform spawnPoint in spawner.transform)
        {
            int itemId = spawnPoint.GetComponent<ItemSpawnInfo>().Id;
            if (_itemPrefabs.ContainsKey(itemId))
            {
                // 아이템 프리팹을 가져옵니다.
                GameObject itemPrefab = _itemPrefabs[itemId];
                // 아이템 프리팹을 인스턴스화합니다.

                GameObject itemObject = Main.Resource.InstantiatePrefab($"{itemPrefab.name}.prefab", spawnPoint.transform);
                Item itemObj = itemObject.AddComponent<Item>();
                // Item itemObj = SceneUtility.GetAddComponent<Item>(itemObject);
                int id = itemPrefab.GetComponent<ItemSpawnInfo>().Id;
                var itemData = FieldItems[id];
                itemObj.Initialize(itemData.Id, itemData.Name, itemData.Category, itemData.Description, itemData.Power, itemData.Duration);

            }
            else
            {
                Debug.LogError($"아이템 프리팹을 찾을 수 없습니다: {itemId}");
            }
        }
    }
}
#endregion
