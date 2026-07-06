using Unity.VisualScripting;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private Vector3 baseWindDirection = new Vector3(1f, 0f, 0f); // 初期の風の向き
    [SerializeField] private float WindForce = 5f; // 風の強さ
    [SerializeField] private float cangeInterval = 5f; // 反転する時間 (秒)

    private SpriteRenderer sprite;

    private Vector3 currentWind;    // 現在の風のベクトル
    private float timer;

    public Vector3 CurrentWind => currentWind;

    private void Start()
    {
        // 初期設定の基本の風向き×強さ
        currentWind = baseWindDirection.normalized * WindForce;
        timer = cangeInterval;
        sprite = GetComponent<SpriteRenderer>();
        

    }

    private void Update()
    {
        timer -= Time.deltaTime;


        if (timer <= 0f)
        {
            currentWind = -currentWind;
            sprite.flipX = !sprite.flipX;
            timer = cangeInterval; // タイマーをリセット
    //      Debug.Log($"風向きが反転しました: {currentWind}");
            
    //      ReverseWind();
        }

        

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject collideObject = collision.gameObject;
        if ( collideObject.CompareTag("Player"))
        {
            
            collideObject.transform.Translate(currentWind * Time.deltaTime, Space.World);
        }
    }
   

    /*
    void ReverseWind()
    {
        // 風の向きを真逆にする
        currentWind = -currentWind;

        // ログで確認
        Debug.Log($"風向きが反転しました！ 現在の風:{currentWind}");

    }
    */
}

   
