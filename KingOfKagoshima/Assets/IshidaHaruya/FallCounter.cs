using UnityEngine;
using UnityEngine.SceneManagement; 

public class FallCounter : MonoBehaviour
{
    public static FallCounter instance;

    
    public int fallCount { get; private set; }

  
    private string currentStageKey = "Stage1_Falls";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 

            
            SceneManager.sceneLoaded += OnSceneLoaded;

            
            UpdateStageKey(SceneManager.GetActiveScene().name);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       
        UpdateStageKey(scene.name);
        
        fallCount = 0;
       
        PlayerPrefs.SetInt(currentStageKey, 0);
        PlayerPrefs.Save();

        Debug.Log($"【{scene.name}】開始：落下回数を0からカウントします。");
    }

    // 箱の名前を自動で作る処理
    private void UpdateStageKey(string sceneName)
    {
        currentStageKey = sceneName + "_Falls";
    }

   
    public void AddFall()
    {
        fallCount++; // カウントを 1 増やす

        
        PlayerPrefs.SetInt(currentStageKey, fallCount);
        PlayerPrefs.Save();

        Debug.Log($"{currentStageKey} の落下回数: {fallCount} 回 (保存完了)");
    }
}