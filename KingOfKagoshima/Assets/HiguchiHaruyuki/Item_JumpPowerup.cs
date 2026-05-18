using System.Collections.Generic;
using UnityEngine;

public class Item_JumpPowerup : ItemBase
{
    [SerializeField] private int m_useCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Name = "ジャンプ力上昇";
        Description = "一定回数ジャンプ力が上がります";
        IconPath = "IconPath.png";
        UseCount = m_useCount;
    }
    //プレイヤー側から呼び出す関数
    public override void Use()
    {
        base.Use();
    }
}
