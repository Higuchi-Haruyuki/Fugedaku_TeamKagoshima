using System;
using System.Collections.Generic;
using UnityEngine;

public class Item_JumpPowerup : ItemBase
{
    [SerializeField] private int m_useCount = 0;
    public float m_jumpPower = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Name = "ジャンプ力上昇";
        Description = "ジャンプ力が上がります";
        IconPath = "ItemIcon/JumpPowerUp";
        UseCount = m_useCount;
    }
    //プレイヤー側から呼び出す関数

    public static float UseItem(PlayerItemSystem playerItemSystem)
    {
        //所持しているなら使用して補正をかける
        var jumpPowerUp = playerItemSystem.CheckItem<Item_JumpPowerup>();
        jumpPowerUp?.Use();
        float jumpPowerModifier = jumpPowerUp == null ? 1 : jumpPowerUp.m_jumpPower;
        return jumpPowerModifier;
    }
    
}
