using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
public class PlayerItemSystem : MonoBehaviour
{
    [SerializeField] private List<ItemBase> m_itemList;
    //デバック用
    private bool m_isPressdSpaceKeyBeforeFlame = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Keyboard.current.aKey.isPressed)
        {
            var pos = transform.position;
            pos.x -= 2 * Time.deltaTime;
            transform.position = pos;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            var pos = transform.position;
            pos.x += 2 * Time.deltaTime;
            transform.position = pos;
        }
        if (Keyboard.current.spaceKey.isPressed)
        {
            if (!m_isPressdSpaceKeyBeforeFlame)
            {
                int jumpPower = 50;
                ItemBase itembase = CheckItem("ジャンプ力上昇");
                //ジャンプ力上昇アイテムをもっているとき
                if (itembase != null) 
                { 
                    var jumpPowerUp = itembase as Item_JumpPowerup; 
                    //キャストにせいこうしたとき
                    if (jumpPowerUp != null) 
                    {
                        jumpPower = jumpPowerUp.m_jumpPower;
                    }
                }

                var pos = transform.position;
                pos.y += jumpPower * Time.deltaTime;
                transform.position = pos;
                m_isPressdSpaceKeyBeforeFlame=true;
            }

        }
        else
        {
            m_isPressdSpaceKeyBeforeFlame = false;
        }*/
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
