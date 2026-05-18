using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            ItemBase item = collision.gameObject.GetComponent<ItemBase>();
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
            }

        }
    }
    //アイテムの名前を引数にとり、所持しているときにTrueを返す関数
    public bool UseItem(string itemName)
    {
        foreach (ItemBase item in m_itemList)
        {
            if(item.Name == itemName)
            {
                item.Use();
                //残り使用回数が0のときアイテムリストから削除する
                if(item.UseCount == 0)
                {
                    m_itemList.Remove(item);
                }
                return true; 
            }
        }
        return false;
    }
}
