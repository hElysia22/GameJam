using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource globalAudio;

    [Header("音效资源")]
    public AudioClip jumpSfx;
    public AudioClip windSfx;
    public AudioSource bgmAudio;
    public AudioClip bgmClip;

    void Start()
    {
        bgmAudio.clip = bgmClip;
        bgmAudio.loop = true;
        bgmAudio.Play();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 单次短音效
    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        globalAudio.PlayOneShot(clip, volume);
    }

    // 循环音效
    public void PlayLoop(AudioClip clip)
    {
        globalAudio.clip = clip;
        globalAudio.Play();
    }

    public void StopLoop()
    {
        globalAudio.Stop();
    }
}
