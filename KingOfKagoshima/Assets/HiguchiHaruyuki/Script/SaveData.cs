using UnityEngine;

[System.Serializable]
public class SaveData
{
    //ステージの番号
    public int StageNum;
    //中断中のプレイのデータ
    public float Time;
    public Vector3 PlayerPos;
    public int JumpCount;
    public int FallCount;
    public override string ToString()
    {
        return $"STAGENUM: {StageNum}, TIME: {Time}, PLAYERPOS: {PlayerPos}, JUMPCOUNT: {JumpCount}, FALLCOUNT: {FallCount}";
    }
}
