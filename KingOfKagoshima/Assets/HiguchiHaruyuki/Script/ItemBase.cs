using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBase : MonoBehaviour
{
    private const int kMaxUseCount = 3;

    private const float kItemRepopTime = 5.0f;

    private const float kInactiveAlpha = 0.3f;

    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string IconPath {  get; protected set; }
    public int UseCount { get => useCount; 
        protected set 
        {
            if(value <= 0)
            { 
                useCount = 0; 
                return; 
            }
            useCount = value;
        } 
    }

    private int useCount;

    private float _itemRepopTimer = 0.0f;
    
    private bool _itemExist = true;

    protected Image _circleGaugeImage;

    public void SetUseCount() => UseCount = kMaxUseCount; 
    public void Use() => UseCount--;


    private void Start()
    {
        _circleGaugeImage = transform.Find("Canvas/RepopCircle").GetComponent<Image>();
        _circleGaugeImage.fillAmount = 0.0f;
    }

    private void Update()
    {
        if(!_itemExist)
        {
            Debug.Log($"itemRepopTimer: {_itemRepopTimer}");
            _itemRepopTimer += Time.deltaTime;

            _circleGaugeImage.fillAmount = _itemRepopTimer / kItemRepopTime;
        }
        if (_itemRepopTimer >= kItemRepopTime)
        {
            _itemExist = true;
            _itemRepopTimer = 0.0f;
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = true;
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer) spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);

            _circleGaugeImage.fillAmount = 0.0f;
        }

        _circleGaugeImage.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    /// <summary>
    /// アイテムが手に入れられたときに呼ぶ関数
    /// </summary>
    public void OnGet()
    {
        var col = GetComponent<Collider2D>();
        if(col) col.enabled = false;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,0.3f);

        _itemExist = false;
    }

}
