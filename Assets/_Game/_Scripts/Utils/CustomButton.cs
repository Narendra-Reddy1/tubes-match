using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CustomButton : Button
{
    [Header("Custom Setting")]
    [SerializeField] private bool clickAnimate = true;


    protected override void Start()
    {
        base.Start();
        transition = Transition.None;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {

        if (clickAnimate && IsInteractable())
        {
            interactable = false;

            gameObject.transform.DOScale(1.05f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                interactable = true;
                base.OnPointerClick(eventData);
            });
        }
        else
        {
            base.OnPointerClick(eventData);
        }

    }


}
