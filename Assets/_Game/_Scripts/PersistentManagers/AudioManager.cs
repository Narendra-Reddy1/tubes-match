using UnityEngine;


public class AudioManager : MonoBehaviour
{

    #region SINGLETON
    public static AudioManager instance { get; private set; }
    #endregion SINGLETON

    [SerializeField] private AudioAsset m_audioAsset;
    private AudioSource m_sfxAudioSource;
    private AudioSource m_bgmAudioSource;
    private void Awake()
    {
        #region SINGLETON
        instance = this;
        #endregion SINGLETON

        _Init();
    }
    private void _Init()
    {
        m_sfxAudioSource = gameObject.AddComponent<AudioSource>();
        m_bgmAudioSource = gameObject.AddComponent<AudioSource>();
        m_sfxAudioSource.playOnAwake = false;
        m_bgmAudioSource.playOnAwake = false;
        m_bgmAudioSource.volume = 0.75f;
        m_bgmAudioSource.loop = true;
    }

    public void PlaySFX(AudioID audioID)
    {
        m_sfxAudioSource.PlayOneShot(m_audioAsset.GetAudioClipByID(audioID));
    }
    public void PlayBGM(AudioID audioID)
    {
        m_bgmAudioSource.clip = m_audioAsset.GetAudioClipByID(audioID);
        m_bgmAudioSource.Play();
    }
}