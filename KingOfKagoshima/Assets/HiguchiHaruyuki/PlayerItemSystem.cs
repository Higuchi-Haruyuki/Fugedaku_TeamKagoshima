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
            ItemBase item = collision.gameObject.GetComponent<ItemBase>();
            if (m_itemList.Count == 0) m_itemList.Add(item);
            else
            {
                //見つかったアイテムを保管する変数
                ItemBase foundItem = null;
                foreach (ItemBase i in m_itemList)
                {
                    //同じアイテムのインスタンスを代入する
                    if (i.Name == item.Name)
                    {
                        foundItem = i; 
                        
                        item.gameObject.GetComponent<Collider2D>().enabled = false;
                        item.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        break;               
                    }
                }
                if (foundItem != null)
                {
                    //アイテムが見つかている時、使用可能数を増加させる
                    foundItem.AddUseCount(item.UseCount);
                }
                else
                {
                    m_itemList.Add(item);
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
                if (item.UseCount == 0)
                {
                    m_itemList.Remove(item);
                    //Destroy(item);
                    return null;
                }
                //残り使用回数が0のときアイテムリストから削除するがインスタンスを返す
                else if (item.UseCount == 1) 
                {
                    m_itemList.Remove(item);
                }
                return item; 
            }
        }
        return null;
    }
    //型引数でアイテムの型を受け取り、所持しているならそのインスタンスを返す
    public T CheckItem<T>() where T : ItemBase
    {
        foreach (ItemBase item in m_itemList)
        {
            if (item is T castedItem)
            {
                //残り使用回数が0のときアイテムリストから削除する
                if (castedItem.UseCount == 0)
                {
                    m_itemList.Remove(castedItem);
                    //Destroy(item);
                    return null;
                }
                //残り使用回数が0のときアイテムリストから削除するがインスタンスを返す
                else if (castedItem.UseCount == 1)
                {
                    m_itemList.Remove(castedItem);
                }
                return castedItem;
            }
            else
            {

            }
        }
        return null;
    }
    public List<ItemBase> GetItems() => m_itemList;
}
