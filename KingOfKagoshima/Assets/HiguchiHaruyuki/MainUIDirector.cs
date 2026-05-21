using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;
public class MainUIDirector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_timerText;
    [SerializeField] private GameObject m_player;

    [SerializeField] private GameObject m_itemsParent;
    [SerializeField] private GameObject m_itemDisplayUIPrefab;
    //アイテム表示UIの1つ目のオフセット
    [SerializeField] private Vector2 m_initialOffset;
    private List<GameObject> m_itemUIList;
    private ScoreTime m_scoreTime;
    private List<ItemBase> m_playerItems;
    private List<int> m_itemUseCountBeforeCall;
    private int itemCountBeforeCall = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_playerItems = m_player.GetComponent<PlayerItemSystem>().GetItems();
        DisplayItem();
    }
    public void SetScoreTime(ScoreTime scoreTime)
    {
        m_scoreTime = scoreTime;
    }
    // Update is called once per frame
    void Update()
    {
        m_timerText.SetText(m_scoreTime?.ToString());
        //アイテム数に変化があったとき（プレイヤーがアイテムを取得または消失したとき）
        if (m_playerItems.Count != itemCountBeforeCall)
        {
            var childCount = m_itemsParent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = m_itemsParent.transform.GetChild(i);
                Destroy(child.gameObject);
            }
            m_itemUIList = new(); 
            m_itemUseCountBeforeCall = new();
            itemCountBeforeCall = 0;
            //描画し直す
            DisplayItem();
        }
        CheckChangeItemUseCount();
    }
    void DisplayItem()
    {
        itemCountBeforeCall = m_playerItems.Count;
        for (int i = 0; i < m_playerItems.Count; i++)
        {
            var offset = m_initialOffset + new Vector2(0,-100 * i);
            var itemUI = Instantiate(m_itemDisplayUIPrefab,m_itemsParent.transform);
            itemUI.transform.localPosition = offset;
            m_itemUIList.Add(itemUI);
            var item = m_playerItems[i];
            //使用可能回数を保存する
            m_itemUseCountBeforeCall.Add(item.UseCount);
            //アイコンの設定
            var image = itemUI.GetComponentInChildren<Image>();
            image.sprite = Resources.Load<Sprite>($"{item.IconPath}");
            //個数表示
            var text = itemUI.GetComponentInChildren<TextMeshProUGUI>();
            text.SetText($"残り{item.UseCount}回");
        }
    }
    //プレイヤーがアイテムを使用していたら描画し直す
    private void CheckChangeItemUseCount()
    {
        for (int i = 0; i < m_playerItems.Count; i++)
        {
            //以前保存されたときと使用可能回数に変化があったとき
            if(m_playerItems[i].UseCount != m_itemUseCountBeforeCall[i])
            {
                var text = m_itemUIList[i].GetComponentInChildren<TextMeshProUGUI>();
                text.SetText($"残り{m_playerItems[i].UseCount}回");
                m_itemUseCountBeforeCall[i] = m_playerItems[i].UseCount;
            }
        }
    }
}
