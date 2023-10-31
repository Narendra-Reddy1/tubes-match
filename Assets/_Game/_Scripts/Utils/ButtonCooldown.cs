using UnityEngine;
using UnityEngine.UI;

namespace BenStudios
{
    [RequireComponent(typeof(Button))]
    public class ButtonCooldown : MonoBehaviour
    {
        [SerializeField] private Button m_button;
        [SerializeField] private float m_coolDownTimer = .75f;
        private void OnEnable()
        {
            if (m_button == null)
                if (TryGetComponent(out m_button))
                {
                    m_button.onClick.AddListener(_CoolDown);
                }
        }
        private void OnDisable()
        {
            m_button?.onClick.RemoveListener(_CoolDown);
        }
        private void _CoolDown()
        {
            m_button.interactable = false;
            Invoke(nameof(StopCoolDown), m_coolDownTimer);
        }
        private void StopCoolDown()
        {

            m_button.interactable = true;
        }

    }
}