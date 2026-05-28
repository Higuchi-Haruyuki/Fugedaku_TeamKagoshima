using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;
public class MainUIDirector : MonoBehaviour
{
    [SerializeField] private int _stageNumber = 1;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject _itemsParent;
    [SerializeField] private GameObject _itemDisplayUIPrefab;
    [SerializeField] private ClearManager _clearManager;
    [SerializeField] private Image fadePanel;
    //アイテム表示UIの1つ目のオフセット
    [SerializeField] private Vector2 _initialOffset;
    // フェードの完了にかかる時間
    [SerializeField] private float fadeDuration = 5.0f; 
    private List<GameObject> _itemUIList;
    private ScoreTime _scoreTime;
    private List<ItemBase> _playerItems;
    private List<int> _itemUseCountBeforeCall;
    private int _itemCountBeforeCall = 0;
    private bool isTimerStop = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerItems = _player.GetComponent<PlayerItemSystem>().GetItems();
        _clearManager.OnClear += OnClear; 
        DisplayItem();
    }
    private void OnDisable()
    {
        _clearManager.OnClear -= OnClear;
    }
    public void SetScoreTime(ScoreTime scoreTime)
    {
        _scoreTime = scoreTime;
    }
    // Update is called once per frame
    void Update()
    {
        if(!isTimerStop)
            _timerText.SetText(_scoreTime?.ToString());
        //アイテム数に変化があったとき（プレイヤーがアイテムを取得または消失したとき）
        if (_playerItems.Count != _itemCountBeforeCall)
        {
            var childCount = _itemsParent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = _itemsParent.transform.GetChild(i);
                Destroy(child.gameObject);
            }
            _itemUIList = new(); 
            _itemUseCountBeforeCall = new();
            _itemCountBeforeCall = 0;
            //描画し直す
            DisplayItem();
        }
        CheckChangeItemUseCount();
    }
    void DisplayItem()
    {
        _itemCountBeforeCall = _playerItems.Count;
        for (int i = 0; i < _playerItems.Count; i++)
        {
            var offset = _initialOffset + new Vector2(0,-100 * i);
            var itemUI = Instantiate(_itemDisplayUIPrefab,_itemsParent.transform);
            itemUI.transform.localPosition = offset;
            _itemUIList.Add(itemUI);
            var item = _playerItems[i];
            //使用可能回数を保存する
            _itemUseCountBeforeCall.Add(item.UseCount);
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
        for (int i = 0; i < _playerItems.Count; i++)
        {
            //以前保存されたときと使用可能回数に変化があったとき
            if(_playerItems[i].UseCount != _itemUseCountBeforeCall[i])
            {
                var text = _itemUIList[i].GetComponentInChildren<TextMeshProUGUI>();
                text.SetText($"残り{_playerItems[i].UseCount}回");
                _itemUseCountBeforeCall[i] = _playerItems[i].UseCount;
            }
        }
    }
     
    public IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        fadePanel.enabled = true;               // パネルを有効化
        float elapsedTime = 0.0f;               // 経過時間を初期化
        Color color = fadePanel.color;
        while (elapsedTime < fadeDuration)
        {

            elapsedTime += Time.deltaTime;                           // 経過時間を増やす                        
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);     // フェードの進行度を計算  
            float i = Mathf.Lerp(0, 1, t);   // パネルの色を変更してフェードアウト
            color.a = i;
            fadePanel.color = color;
            yield return null;                                       // 1フレーム待機
        }
        color.a = 1;
        fadePanel.color = color;
        SceneManager.LoadScene(sceneName);
    }
    private void OnClear()
    {
        isTimerStop = true;
        //時間の保存
        _scoreTime.SaveTime(_stageNumber);
        //最速タイムの保存
        _scoreTime.SaveFastestTime(_stageNumber);
        //シーンのロード
        StartCoroutine(FadeOutAndLoadScene("HaruyukiResultScene"));
    }
}
