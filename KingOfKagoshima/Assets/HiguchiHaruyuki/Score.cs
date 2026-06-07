using System;
using UnityEngine;

public class Score
{
    public int Hours { get; private set; }
    public int Minutes { get; private set; }
    public int Seconds { get; private set; }
    public int Milliseconds { get; private set; }
    public int JumpCount { get; set; }
    public int FallCount { get; set; }
    private float m_seconds;
    public float GetSeconds()
    {
        return m_seconds;
    }
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
    //最速タイムを保存する関数
    public void SaveFastestTime()
    {
        PlayerPrefs.SetFloat("FastestTime", m_seconds);
    }
    //最速タイム
    public void LoadFastestTime()
    {
        m_seconds = PlayerPrefs.GetFloat("FastestTime");
    }
    //ステージのタイムを保存（上書きされる）
    public void SaveTime()
    {
        PlayerPrefs.SetFloat($"Time", m_seconds);
    }
    //ステージのタイムを取得
    public void LoadTime()
    {
        m_seconds = PlayerPrefs.GetFloat("Time");
        TimeSpan span = TimeSpan.FromSeconds((double)m_seconds);
        Hours = span.Hours;
        Minutes = span.Minutes;
        Seconds = span.Seconds;
        Milliseconds = span.Milliseconds;
    }
    //ステージのタイムを取得
    public void LoadTime(int num)
    {
        Debug.Log("無効だから書き換えてね");
    }
    //ジャンプ回数を保存
    public void SaveJumpCount()
    {
        PlayerPrefs.SetInt("JumpCount",JumpCount);

    }
    //ジャンプ回数を取得
    public void LoadJumpCount()
    {
        JumpCount = PlayerPrefs.GetInt("JumpCount");
    }
    //落下回数を保存
    public void SaveFallCount()
    {
        PlayerPrefs.SetInt("FallCount", FallCount);
    }
    //落下回数を取得
    public void LoadFallCount()
    {
        FallCount = PlayerPrefs.GetInt("FallCount");
    }
    public void SaveAll()
    {
        SaveTime();
        SaveJumpCount();
        SaveFallCount();
    }
    public void LoadAll()
    {
        LoadTime();
        LoadJumpCount();
        LoadFallCount();
    }
}
