using UnityEngine;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;
public enum SaveFile
{
    Stage1SaveData,Stage2SaveData,Stage1FastestTimeData,Stage2FastestTimeData
}

public class SaveManager
{                   
    public static void SaveJson(SaveData data,string path)
    {
        string json = JsonUtility.ToJson(data);                 // jsonとして変換
        StreamWriter wr = new StreamWriter(path, false);    // ファイル書き込み指定
        wr.WriteLine(json);
        Debug.Log($"save {path}");
        wr.Close();
    }
    public static SaveData LoadJson(string path)
    {
        if(!File.Exists(path))
        {
            SaveJson(new(), path);
        }
        StreamReader rd = new StreamReader(path);               // ファイル読み込み指定
        string json = rd.ReadToEnd();
        Debug.Log($"load {path}");
        rd.Close();                                             // ファイル閉じる

        return JsonUtility.FromJson<SaveData>(json);            // jsonファイルを型に戻して返す
    }

    public static string GetPath(SaveFile saveFile)
    {
        switch (saveFile)
        {
            case SaveFile.Stage1SaveData:
                return Application.dataPath + "/" + "Stage1SaveData.json";
            case SaveFile.Stage2SaveData:
                return Application.dataPath + "/" + "Stage2SaveData.json";
            case SaveFile.Stage1FastestTimeData:
                return Application.dataPath + "/" + "Stage1FastestTimeData.json";
            case SaveFile.Stage2FastestTimeData:
                return Application.dataPath + "/" + "Stage2FastestTimeData.json";
            default:
                return "";
        }
    }
}
