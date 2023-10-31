using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TubeHandler : MonoBehaviour
{
    [SerializeField] private BallEntity _ballPrefab;
    [SerializeField] private ColorsDataBase _colorsDatabase;
    [SerializeField] private Transform _sphereBallsParent;
    [SerializeField] private Transform _tweenPose;
    [SerializeField] private TextMeshPro _ballCountTxt;
    [SerializeField] private List<TubeBallHolder> _tubeBallHolders = new List<TubeBallHolder>();
    [SerializeField] private int _capacityOfTheTube = 3;
    [SerializeField] private int _sizeOfTheTube = 5;
    [SerializeField] int _index = 2;
    private Queue<BallEntity> _ballEntityQueue = new Queue<BallEntity>();
    private int _removedBallCount = 0;

    private void Start()
    {
        _Init();
    }
    private void _Init()
    {
        _UpdateBallsCount(_sizeOfTheTube);
        for (int i = 0; i < _sizeOfTheTube; i++)
        {
            BallEntity entity = Instantiate(_ballPrefab, _sphereBallsParent);
            entity.Init(_index, _colorsDatabase.GetColoredMaterialAtIndex(_index), this);
            entity.transform.localScale = Vector3.zero;
            _ballEntityQueue.Enqueue(entity);
        }
        StartCoroutine(_SpawnColoredBalls());
    }

    private IEnumerator _SpawnColoredBalls()
    {
        for (int i = 0; i < _capacityOfTheTube; i++)
        {
            BallEntity entity = _ballEntityQueue.Dequeue();
            _tubeBallHolders[i].Setup(entity);

        }
        yield return _waitForSeconds;
        _tubeBallHolders[0].ActivateEntity();
    }
    WaitForSeconds _waitForSeconds = new WaitForSeconds(.6f);

    private void _UpdateBallsCount(int count)
    {
        _ballCountTxt.SetText(count.ToString());
    }

    public void RemoveTheEntity(BallEntity entity)
    {
        _tubeBallHolders.Find(x => x.referencedEntity == entity).Reset();
        _removedBallCount++;
        _UpdateBallsCount(_sizeOfTheTube - _removedBallCount);
    }
    public bool IsTubeEmpty()
    {
        return (_sizeOfTheTube - _removedBallCount) == 0;
    }
    public void TweenToTopPose(BallEntity entity, System.Action onComplete = null)
    {
        entity.transform.DOMove(_tweenPose.position, .2f).onComplete += () =>
        {
            onComplete?.Invoke();
            RemoveTheEntity(entity);
            ReArrangeSphereBalls();
        };
    }
    public void ReArrangeSphereBalls()
    {
        for (int j = 0; j < _tubeBallHolders.Count - 1; j++)
        {
            if (!_tubeBallHolders[j].isOccupied)
            {
                _tubeBallHolders[j].Setup(_tubeBallHolders[j + 1].referencedEntity);
                _tubeBallHolders[j + 1].Reset();
            }
        }
        _FillEmptyHolder();
        MyUtils.DelayedCallback(.12f, ActivateTopBall);
    }
    private void _FillEmptyHolder()
    {
        if (_ballEntityQueue.Count <= 0) return;
        _tubeBallHolders.Find(x => !x.isOccupied).Setup(_ballEntityQueue.Dequeue());
    }
    private void ActivateTopBall()
    {
        _tubeBallHolders[0].referencedEntity?.Activate();
    }

    [System.Serializable]
    public class TubeBallHolder
    {
        public Transform holder;
        public BallEntity referencedEntity;
        public bool isOccupied;
        public void Setup(BallEntity entity)
        {
            referencedEntity = entity;
            isOccupied = true;
            entity?.transform.DOScale(Vector3.one * 1.5f, .2f);
            TweenToPose();
        }
        public void Reset()
        {
            referencedEntity = null;
            isOccupied = false;
        }
        public void ActivateEntity()
        {
            referencedEntity?.Activate();
        }
        public void TweenToPose()
        {
            referencedEntity?.transform.DOMove(holder.position, .1f);
        }
    }
}
