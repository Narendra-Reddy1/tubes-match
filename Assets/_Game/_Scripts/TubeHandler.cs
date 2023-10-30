using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TubeHandler : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private int _capacityOfTheTube = 3;
    [SerializeField] private int _sizeOfTheTube = 5;
    [SerializeField] private ColorsDataBase _colorsDatabase;
    [SerializeField] private TextMeshPro _ballCountTxt;
    private Queue<BallEntity> _ballEntityQueue;
    private int _currIndex = 0;
    private void Start()
    {
        _Init();
    }
    private void _Init()
    {
        _ballCountTxt.SetText(_sizeOfTheTube.ToString());
    }

    private void _SpawnColoredBalls()
    {

    }

    public class TubeBallHolder
    {
        public Transform holder;
        public BallEntity referencedEntity;
        public bool isOccupied;
        public void TweenToPose()
        {

        }
    }
}
