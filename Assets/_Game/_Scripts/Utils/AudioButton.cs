using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class AudioButton : MonoBehaviour
{
    [SerializeField] private AudioID _audioID = AudioID.ButtonClickSFX;
    private Button _button;
    private void OnEnable()
    {
        if (TryGetComponent(out _button))
        {
            _button.onClick.AddListener(PlaySFX);
        }
    }
    private void OnDisable()
    {
        _button?.onClick.RemoveListener(PlaySFX);
    }
    private void PlaySFX()
    {
        AudioManager.instance.PlaySFX(_audioID);
    }
}