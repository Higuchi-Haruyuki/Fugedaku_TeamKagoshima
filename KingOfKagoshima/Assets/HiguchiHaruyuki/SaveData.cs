using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int StageNum;
    //中断中のプレイのデータ
    public float Time;
    public Vector3 PlayerPos;
    public int JumpCount;
    public int FallCount;
}
