using UnityEngine;

public class IceFloor : MonoBehaviour
{
    public AudioClip iceSound;

    public AudioSource audioSource;

//    private Rigidbody2D playerRigidbody;

    private bool isPlayerOn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 氷の床についてるAudioSourseを取得
        audioSource = GetComponent<AudioSource>();

        // AudioSourseの初期設定
        if (audioSource != null )
        {
            audioSource.playOnAwake = false;    // ゲーム開始時に勝手にならないようにする
        }


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isPlayerOn)
        {
            isPlayerOn = true; // 乗った状態にする
            if (audioSource != null && iceSound != null)
            {
                audioSource.PlayOneShot( iceSound );
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = false;
        }
    }










    // Update is called once per frame
    void Update()
    {
        
    }
}
