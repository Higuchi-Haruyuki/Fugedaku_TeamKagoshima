using UnityEngine;

[System.Serializable]
public class SaveData
{
    //Stage1中断中のプレイのデータ
    public float Stage1Time;
    public Vector3 Stage1PlayerPos;
    public int Stage1JumpCount;
    public int Stage1FallCount;
    //Stage1ハイスコアのデータ
    public float Stage1HightScoreTime;
    //Stage2中断中のプレイのデータ
    public float Stage2Time;
    public Vector3 Stage2PlayerPos;
    public int Stage21JumpCount;
    public int Stage21FallCount;
    //Stage2ハイスコアのデータ
    public float Stage2HightScoreTime;
}
