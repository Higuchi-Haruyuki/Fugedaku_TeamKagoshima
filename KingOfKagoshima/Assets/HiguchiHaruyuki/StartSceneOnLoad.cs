using UnityEngine;

public class StartSceneOnLoad : MonoBehaviour
{
    public int StageNum;
    [SerializeField] private GameObject _player;
    [SerializeField] private UIDirector _uIDirector;
    private SaveData _data;
    private void Awake()
    {
        if (StageNum == 1)
            _data = SaveManager.LoadJson(SaveManager.GetPath(SaveFile.Stage1SaveData));
        else if (StageNum == 2)
            _data = SaveManager.LoadJson(SaveManager.GetPath(SaveFile.Stage2SaveData));
        _player.transform.position = _data.PlayerPos;
        _uIDirector.SetScore(_data);
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
