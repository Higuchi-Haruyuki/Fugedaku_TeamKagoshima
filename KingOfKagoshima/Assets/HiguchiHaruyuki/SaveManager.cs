using UnityEngine;
using System.IO;
public class SaveManager : MonoBehaviour
{
    [HideInInspector] public SaveData data;     // json変換するデータのクラス
    private string _filepath;                            // jsonファイルのパス
    private string _fileName = "SaveData.json";              // jsonファイル名
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // パス名取得
        _filepath = Application.dataPath + "/" + _fileName;

        // ファイルがないとき、ファイル作成
        if (!File.Exists(_filepath))
        {
            SaveJson(new());
        }

        // ファイルを読み込んでdataに格納
        data = LoadJson();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SaveJson(SaveData data)
    {
        string json = JsonUtility.ToJson(data);                 // jsonとして変換
        StreamWriter wr = new StreamWriter(_filepath, false);    // ファイル書き込み指定
        wr.WriteLine(json);                                     // json変換した情報を書き込み
        wr.Close();
    }
    SaveData LoadJson()
    {
        StreamReader rd = new StreamReader(_filepath);               // ファイル読み込み指定
        string json = rd.ReadToEnd();                           // ファイル内容全て読み込む
        rd.Close();                                             // ファイル閉じる

        return JsonUtility.FromJson<SaveData>(json);            // jsonファイルを型に戻して返す
    }
}
