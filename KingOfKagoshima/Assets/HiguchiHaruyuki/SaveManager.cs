using UnityEngine;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;
public class SaveManager : MonoBehaviour
{
    private const int STAGE_COUNT = 2;
    private List<string> _filepaths;                            // jsonファイルのパス
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _filepaths = new();
        for (int i = 1; i <= STAGE_COUNT; i++)
        {
            // パス名取得
            _filepaths.Add(Application.dataPath + "/" + $"Stage{i}SaveData.json");
        }
        for (int i = 1; i <= STAGE_COUNT; i++)
        {
            // パス名取得
            _filepaths.Add(Application.dataPath + "/" + $"Stage{i}FastestTimeData.json");
        }
        for (int i = 0; i < STAGE_COUNT * 2; i++)
        {
            // パス名取得
            // ファイルがないとき、ファイル作成
            if (!File.Exists(_filepaths[i]))
            {
                SaveJson(new(),i);
            }

        }
    }

    //idx0: stage1SaveData
    //idx1: stage2SaveData
    //idx2: stage1FastestTimeData
    //idx3: stage2FastestTimeData
    public void SaveJson(SaveData data,int idx)
    {
        string json = JsonUtility.ToJson(data);                 // jsonとして変換
        StreamWriter wr = new StreamWriter(_filepaths[idx], false);    // ファイル書き込み指定
        wr.WriteLine(json);                                     // json変換した情報を書き込み
        wr.Close();
    }
    //idx0: stage1SaveData
    //idx1: stage2SaveData
    //idx2: stage1FastestTimeData
    //idx3: stage2FastestTimeData
    public SaveData LoadJson(int idx)
    {
        StreamReader rd = new StreamReader(_filepaths[idx]);               // ファイル読み込み指定
        string json = rd.ReadToEnd();                           // ファイル内容全て読み込む
        rd.Close();                                             // ファイル閉じる

        return JsonUtility.FromJson<SaveData>(json);            // jsonファイルを型に戻して返す
    }
}
