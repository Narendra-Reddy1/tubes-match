using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallEntity : MonoBehaviour, IPointerDownHandler//,IPointerClickHandler,IPointerUpHandler
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private TubeHandler _myParentTube;
    private bool canSelect = false;
    private int _id;
    public int Id => _id;


    public void Init(int index, Material mat, TubeHandler parentHandler)
    {
        _id = index;
        _meshRenderer.sharedMaterial = mat;
        _trailRenderer.material = mat;
        _myParentTube = parentHandler;
    }
    public void Activate()
    {
        canSelect = true;
        _AnimateBall();
    }
    public void DeActivate()
    {
        canSelect = false;
        transform.DOKill();
    }

    public void ToggleTrail(bool toggle)
    {
        _trailRenderer.enabled = toggle;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canSelect || Input.touchCount > 1) return;
        DeActivate();
        AudioManager.instance.PlaySFX(AudioID.BallClickSFX);
        GlobalEventHandler.OnBallEntitySelected?.Invoke(this);
    }
    public void ShrinkAndDestroy()
    {
        transform.DOScale(Vector3.zero, .1f).SetUpdate(true).onComplete += () =>
        {
            Destroy(gameObject);
        };
    }

    public void TweenToTheTop(System.Action onComplete = null)
    {
        transform.DOKill();
        _myParentTube.TweenToTopPose(this, onComplete);
    }

    private void _AnimateBall()
    {
        transform.DOMoveY(transform.position.y + 0.2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //}
}
