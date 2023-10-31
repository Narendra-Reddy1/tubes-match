using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenStudios.ScreenManagement
{
    public class PopupBase : WindowBase
    {
        [Header("Popup Content")]
        [SerializeField] private Transform popupPanel;
        [SerializeField] private CanvasGroup transparentBGCanvasGroup;
        [SerializeField] private CanvasGroup popupPanelCanvasGroup;
        [Tooltip("It's a static event. Make sure the method binding to it have generic data")]
        public static Action onPopupClosed;


        #region UnityMethods
        public virtual void OnEnable()
        {
            if (popupPanel.GetComponent<CanvasGroup>() != null)
            {
                popupPanelCanvasGroup = popupPanel.GetComponent<CanvasGroup>();
                StartAnimation();
            }
            else
            {
                Debug.LogError("ComponentMissing : CanvasGroup component missing on popupPanel" + this.gameObject.name);
            }
        }

        public virtual void OnDisable()
        {
        }

        #endregion UnityMethods

        #region PublicMethods
        public override void EndAnimation(Action OnAnimationComplete)
        {
            var sequence = DOTween.Sequence();
            //sequence.Join(popupPanel.DOLocalMoveY(250, 0.25f).SetEase(Ease.InBack));
            sequence.Join(popupPanel.DOScale(0, 0.25f).SetEase(Ease.InBack));
            sequence.Join(popupPanelCanvasGroup.DOFade(0, 0.25f).SetEase(Ease.InBack));
            sequence.Join(transparentBGCanvasGroup?.DOFade(0, 0.25f).SetEase(Ease.InBack));
            sequence.SetUpdate(true);
            sequence.OnComplete(() =>
            {
                OnAnimationComplete();
            });
        }

        public override void StartAnimation()
        {
            //popupPanel.localPosition = new Vector3(0, 250, 0);
            popupPanel.localScale = Vector3.zero;
            popupPanelCanvasGroup.alpha = 0;
            transparentBGCanvasGroup.alpha = 0;

            var sequence = DOTween.Sequence();
            //sequence.Join(popupPanel.DOLocalMoveY(0, 0.4f).SetEase(Ease.OutBack));
            sequence.Join(popupPanel.DOScale(1, 0.4f).SetEase(Ease.OutBack));
            sequence.Join(popupPanelCanvasGroup.DOFade(1, 0.4f).SetEase(Ease.OutBack));
            sequence.Join(transparentBGCanvasGroup?.DOFade(1, 0.3f).SetEase(Ease.OutBack));
            sequence.SetUpdate(true);
        }

        public virtual bool ShowPopup()
        {
            return false;
        }
        public override void OnCloseClick()
        {
        }
        #endregion PublicMethods
    }
}