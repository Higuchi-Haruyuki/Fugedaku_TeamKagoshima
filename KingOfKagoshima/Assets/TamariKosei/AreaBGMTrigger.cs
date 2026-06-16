
using UnityEngine;

public class AreaBGMTrigger : MonoBehaviour
{
    [Header("このエリアで流したいBGM")]
    [SerializeField] private AudioClip climbingBGM;

    [Header("音量")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    [SerializeField] private bool isLoop = false;

    private static AudioSource GetAudioSource;

    private void Awake()
    {
        if (GetAudioSource == null)
        {
            GameObject bgmObj = new GameObject("GlobalBGMPlayer");
            GetAudioSource = bgmObj.AddComponent<AudioSource>();
            GetAudioSource.loop = isLoop;
            // シーンが変わっても消したくない場合は以下をお好みに
            // DontDestroyOnLoad(bgmObj)
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            GetAudioSource.Stop();
            if (collider.GetComponent<Rigidbody2D>().linearVelocityY > 0)
            {
                if (GetAudioSource.clip == climbingBGM && GetAudioSource.isPlaying)
                {
                    return;
                }
                GetAudioSource.clip = climbingBGM;
                GetAudioSource.volume = volume;
                GetAudioSource.loop = isLoop;
                GetAudioSource.Play();

                Debug.Log($"BGMを切り替えたで:{climbingBGM.name}");

            }
        }
    }
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetAudioSource.Stop();
        }
    }*/

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
