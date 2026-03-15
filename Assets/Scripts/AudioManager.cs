using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip jumpClip;
    public AudioClip crashClip;
    public AudioClip scoreMilestoneClip;
    public AudioClip newRecordClip;
    public AudioClip uiClickClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Thêm dòng này để âm thanh không bị ngắt khi chơi lại
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM()
    {
        if (bgmSource.clip != bgmClip)
        {
            bgmSource.clip = bgmClip;
        }
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}