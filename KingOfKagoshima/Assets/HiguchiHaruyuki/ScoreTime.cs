using System;
using UnityEngine;

public static class ScoreTime
{
    public static int Hours { get; set; }
    public static int Minutes { get; set; }
    public static int Seconds { get; set; }
    public static int Milliseconds { get; set; }
    private static float m_seconds;
    public static void AddTime(float time)
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
    public static new string ToString()
    {
        return string.Format("{0:D2}h{1:D2}m{2:D2}s{3:D3}ms",
            Hours, Minutes, Seconds, Milliseconds);
    }
}
