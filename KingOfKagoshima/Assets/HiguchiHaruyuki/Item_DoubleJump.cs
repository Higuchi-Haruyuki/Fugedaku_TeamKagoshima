using UnityEngine;

public class Item_DoubleJump : ItemBase
{
    [SerializeField] private int m_useCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Name = "二段ジャンプ";
        Description = "一定回数二段ジャンプができます";
        IconPath = "IconPath.png";
        UseCount = m_useCount;
    }
    //プレイヤー側から呼び出す関数 
    public override ItemBase Use()
    {
        base.Use();
        return this;
    }

}
