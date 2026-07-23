using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string IconPath {  get; protected set; }
    public int UseCount { get => useCount; 
        protected set 
        {
            if (useCount == 0)
                _itemExist = false;
            useCount = value;
        } 
    }
    public void AddUseCount(int useCount) => UseCount = useCount; 
    public void Use() => UseCount--;

    private float _itemRepopTimer = 0.0f;
    private bool _itemExist = true;
    private int useCount;
    private const float kItemRepopTime = 10.0f;
    private void Update()
    {
        if(!_itemExist)
        {
            _itemRepopTimer += Time.deltaTime;
        }
        if (_itemRepopTimer >= kItemRepopTime)
        {
            _itemExist = true;
            _itemRepopTimer = 0.0f;
        }
    }
}
