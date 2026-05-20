using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
public class PlayerItemSystem : MonoBehaviour
{
    [SerializeField] private List<ItemBase> m_itemList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Item"))
        {
            Debug.Log("Itemと衝突しました");
            ItemBase item = collision.gameObject.GetComponent<ItemBase>();
            if (m_itemList.Count == 0) m_itemList.Add(item);
            else
            {
                foreach (ItemBase i in m_itemList)
                {
                    //同じアイテムをもっていないときアイテムリストに追加する
                    if (i.Name != item.Name)
                    {
                        m_itemList.Add(item);                    
                    }
                    //持っているときは既存の使用回数を増加させる
                    else
                    {
                        i.AddUseCount(item.UseCount);
                    }
                    item.gameObject.GetComponent<Collider2D>().enabled = false;
                    item.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
    }
    //アイテムの名前を引数にとり、所持しているときに使用して、そのインスタンスを返す関数
    public ItemBase CheckItem(string itemName)
    {
        foreach (ItemBase item in m_itemList)
        {
            if(item.Name == itemName)
            {                
                //残り使用回数が0のときアイテムリストから削除する
                if(item.UseCount == 0)
                {
                    m_itemList.Remove(item);
                    Destroy(item);
                    return null;
                }
                return item; 
            }
        }
        return null;
    }
}
