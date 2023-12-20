using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    private ItemSpawnInfo _itemSpawnInfo;
    private int _id;
    private float fallTime = 5f;
    private Item item;
    // Start is called before the first frame update
   private void Start()
    {
        Main.Obstacle.OnInitializedObstacle += InitializedObstacle;
        _itemSpawnInfo = gameObject.GetComponent<ItemSpawnInfo>();
        _id = _itemSpawnInfo.Id;
        item = gameObject.GetComponent<Item>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
               // StartCoroutine(Fall(fallTime));
                
                Main.Item.AddItem(item);
            }
        }
    }
    private IEnumerator Fall(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    private void InitializedObstacle()
    {
        gameObject.SetActive(true);
    }
}
