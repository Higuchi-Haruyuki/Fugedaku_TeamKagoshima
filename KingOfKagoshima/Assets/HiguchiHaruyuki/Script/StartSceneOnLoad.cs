using UnityEngine;

public class StartSceneOnLoad : MonoBehaviour
{
    [SerializeField] private int StageNum;
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private GameObject _player;
    [SerializeField] private UIDirector _uIDirector;
    private SaveData _data;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (StageNum == 1)
            _data = SaveManager.LoadJson(SaveManager.GetPath(SaveFile.Stage1SaveData));
        else if (StageNum == 2)
            _data = SaveManager.LoadJson(SaveManager.GetPath(SaveFile.Stage2SaveData));

        //新規作成されたデータのとき
        if(_data.Time  == 0)
            _player.transform.position = _startPos;
        else
            _player.transform.position = _data.PlayerPos;

        _uIDirector.SetData(_data);
        _uIDirector.SetStageNum(StageNum);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
