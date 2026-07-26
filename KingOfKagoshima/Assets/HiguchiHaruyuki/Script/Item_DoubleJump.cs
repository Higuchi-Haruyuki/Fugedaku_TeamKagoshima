using UnityEngine;
using UnityEngine.UI;


public class Item_DoubleJump : ItemBase
{
    [SerializeField] private int m_useCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _circleGaugeImage = transform.Find("Canvas/RepopCircle").GetComponent<Image>();
        _circleGaugeImage.fillAmount = 0.0f;

        Name = "二段ジャンプ";
        Description = "空中でもう1回ジャンプができます";
        IconPath = "ItemIcon/DoubleJump";
        UseCount = m_useCount;
    }
    //プレイヤー側から呼び出す関数 
    public static bool UseItem(PlayerItemSystem playerItemSystem)
    {
        if (playerItemSystem.CheckItem<Item_DoubleJump>() is var doubleJump && doubleJump != null)
        {
            doubleJump.Use();
            return true;
        }
        return false;
    }

}
