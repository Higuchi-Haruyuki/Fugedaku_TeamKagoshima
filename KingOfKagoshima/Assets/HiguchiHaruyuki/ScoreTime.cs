using System;
using UnityEngine;

public class ScoreTime
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public int Milliseconds { get; set; }
    private float m_seconds;
    public void AddTime(float time)
    {
        m_seconds += time;
        // TimeSpanを使って、秒数から各単位を自動計算
        // floatの精度問題に対応するため、一旦doubleで扱うのが安全
        TimeSpan span = TimeSpan.FromSeconds((double)m_seconds);

        Hours = span.Hours;
        Minutes = span.Minutes;
        Seconds = span.Seconds;
        Milliseconds = span.Milliseconds; // ここでミリ秒が取れる
    }
    public override string ToString()
    {
        return string.Format("{0:D2}h{1:D2}m{2:D2}s{3:D3}ms",
            Hours, Minutes, Seconds, Milliseconds);
    }
    public bool Compare(ScoreTime scoreTime)
    {
        if (Hours > scoreTime.Hours) return true;
        else if (Hours < scoreTime.Hours) return false;
        if(Minutes > scoreTime.Minutes) return true;
        else if (Minutes < scoreTime.Minutes) return false;
        if (Seconds > scoreTime.Seconds) return true;
        else if (Seconds < scoreTime.Seconds) return false;
        if (Milliseconds > scoreTime.Milliseconds) return true;
        else if (Milliseconds < scoreTime.Milliseconds) return false;
        return true;
    }
    //ステージのタイムを現在の最速タイムと比較して早いほうを保存し、更新されたらtrueを返す関数
    public bool SaveFastestTime(int stageNumber)
    {
        ScoreTime currentHSco = new ScoreTime();

        currentHSco.Hours = PlayerPrefs.GetInt($"Stage{stageNumber}FastestTimeHours");
        currentHSco.Minutes = PlayerPrefs.GetInt($"Stage{stageNumber}FastestTimeMinutes");
        currentHSco.Seconds = PlayerPrefs.GetInt($"Stage{stageNumber}FastestTimeSeconds");
        currentHSco.Milliseconds = PlayerPrefs.GetInt($"Stage{stageNumber}FastestTimeMilliseconds");

        if (Compare(currentHSco))
        {
            PlayerPrefs.SetInt($"Stage{stageNumber}FastestTimeHours", Hours);
            PlayerPrefs.SetInt($"Stage{stageNumber}FastestTimeMinutes", Minutes);
            PlayerPrefs.SetInt($"Stage{stageNumber}FastestTimeSeconds", Seconds);
            PlayerPrefs.SetInt($"Stage{stageNumber}FastestTimeMilliseconds", Milliseconds);
            return true;
        }
        return false;
    }
    //ステージのタイムを最速タイムから取得
    public void LoadFastestTime(int stageNumber)
    {
        Hours = PlayerPrefs.GetInt($"Stage{stageNumber}FastestTimeHours");
        Minutes = PlayerPrefs.GetInt($"Stage{stageNumber}FastestTimeMinutes");
        Seconds = PlayerPrefs.GetInt($"Stage{stageNumber}FastestTimeSeconds");
        Milliseconds = PlayerPrefs.GetInt($"Stage{stageNumber}FastestTimeMilliseconds");
    }
    //ステージのタイムを保存（上書きされる）
    public void SaveTime(int stageNumber)
    {
        PlayerPrefs.SetInt($"Stage{stageNumber}TimeHours", Hours);
        PlayerPrefs.SetInt($"Stage{stageNumber}TimeMinutes", Minutes);
        PlayerPrefs.SetInt($"Stage{stageNumber}TimeSeconds", Seconds);
        PlayerPrefs.SetInt($"Stage{stageNumber}TimeMilliseconds", Milliseconds);
    }
    //ステージのタイムを取得
    public void LoadTime(int stageNumber)
    {
        Hours = PlayerPrefs.GetInt($"Stage{stageNumber}TimeHours");
        Minutes = PlayerPrefs.GetInt($"Stage{stageNumber}TimeMinutes");
        Seconds = PlayerPrefs.GetInt($"Stage{stageNumber}TimeSeconds");
        Milliseconds = PlayerPrefs.GetInt($"Stage{stageNumber}TimeMilliseconds");
    }
}
