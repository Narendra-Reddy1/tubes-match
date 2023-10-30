using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchingAreaManager : MonoBehaviour
{
    [SerializeField] private List<BallHolder> _ballHolders;
    [SerializeField] private ParticleSystem _backupParticleEffect;
    

    private List<BallEntity> _collectedEntities = new List<BallEntity>();
    private int _index = 0;

    private void OnEnable()
    {
        GlobalEventHandler.OnBallEntitySelected += Callback_On_Ball_Selected;
    }
    private void OnDisable()
    {
        GlobalEventHandler.OnBallEntitySelected -= Callback_On_Ball_Selected;
    }


    private void AddObjectToBottomCollection(BallEntity entity)
    {
        _index = GetIndexForEntry(entity);
        if (_index < 0 || _index > _collectedEntities.Count)
        {
            Debug.LogError($"INDEX IS INVALID");
            return;
        }

        //DeftouchUtils.Log($"$${entity.gameObject.name} : Index: {index}");
        if (_index != _collectedEntities.Count)
            RearrangeObjectsOnInsert(_index);
        _ballHolders[_index].SetDataFromEntity(entity);
        //entity.DisableInteraction();


        List<int> matchedObjectIndices = new List<int>();
        List<BallEntity> matchedObjects = new List<BallEntity>();



        //int layerIndex = LayerMask.NameToLayer(collectedObjectsLayer);
        //entity.gameObject.layer = layerIndex;
        //entity.transform.GetChild(0).gameObject.layer = layerIndex;
        _collectedEntities.Insert(_index, entity);

        //DeftEventHandler.TriggerEvent(EventID.REQUEST_OBJECT_ENTITY_ANIMATION_RESET);
        ShowRearrangeAnimation();

        //DeftEventHandler.TriggerEvent(EventID.EVENT_OBJECT_ADDED_TO_BOTTOM_COLLECTION, null);

        bool isMatchFound = CheckForTripleMatch(entity, matchedObjects, matchedObjectIndices);
        entity.TweenToTheTop(() =>
        {
            TweenToBottomCollection(entity, _ballHolders[_index].holder.position, 0.2f, () =>
            {
                // entity.transform.DOScale(Vector3.one, .1f).SetUpdate(true);
                if (isMatchFound)
                    ShowObjectMatchAnimation(matchedObjects, matchedObjectIndices);
                else
                    CheckForDefeat();
                //DeftEventHandler.TriggerEvent(EventID.REQUEST_OBJECT_ENTITY_ANIMATION);
            });
        });

    }


    private bool CheckForTripleMatch(BallEntity newlyAddedEntity, List<BallEntity> matchedObjects, List<int> matchedObjectIndices)
    {
        bool matchFound = false;
        int counter = 0;
        int matchID = newlyAddedEntity.Id;
        _collectedEntities.ForEach((x) =>
        {
            if (x.Id.Equals(matchID))
                counter++;
        });
        if (counter < 3)
        {
            return matchFound;
        }
        matchFound = true;
        counter = 3;

        matchedObjects.Clear();
        matchedObjectIndices.Clear();
        //Removing From Object Holders
        //matchedObjects.Capacity = DeftouchConfig.MIN_OBJECTS_TO_MATCH;
        // matchedObjectIndices.Capacity = DeftouchConfig.MIN_OBJECTS_TO_MATCH;
        matchedObjects.Add(newlyAddedEntity);
        matchedObjectIndices.Add(_collectedEntities.IndexOf(newlyAddedEntity));
        for (int i = 0; i < _collectedEntities.Count; i++)
        {
            if (_collectedEntities[i] != newlyAddedEntity && _collectedEntities[i].Id.Equals(matchID))
            {
                matchedObjects.Add(_collectedEntities[i]);
                matchedObjectIndices.Add(i);
            }
        }
        matchedObjects.TrimExcess();
        matchedObjectIndices.TrimExcess();
        for (int i = 0; i < counter; i++)
        {
            BallEntity obj = _collectedEntities.Find(x => x.Id.Equals(matchID));
            _collectedEntities.Remove(obj);
            _ballHolders.Find(x => x.id.Equals(matchID)).ResetHolder();
        }

        //DeftEventHandler.TriggerEvent(EventID.EVENT_ON_MATCH_FOUND, matchedObjects);
        //if (IsBottomCollectionEmty())
        //{
        //    if (GlobalVariables.HighestUnlockedLevel != 1)
        //        DeftouchUtils.DelayedCallback(1.5f, () => { DeftEventHandler.TriggerEvent(EventID.EVENT_ON_BOTTOM_COLLECTION_EMPTY); });//dummy delay
        //}
        RearrangeObjectsOnDeletion(matchedObjectIndices.Min());
        return matchFound;
    }


    private void RearrangeObjectsOnDeletion(int startIndex = 0)
    {
        for (int i = startIndex; i < _ballHolders.Count; i++)
        {
            if (!_ballHolders[i].isOccupied)
            {
                for (int j = i + 1; j < _ballHolders.Count; j++)
                {
                    if (_ballHolders[j].isOccupied)
                    {
                        AddObjectsToNewIndex(j, i);
                        break;
                    }
                }
            }
        }
    }

    private void CheckForDefeat()
    {
        if (_collectedEntities.Count >= _ballHolders.Count)
        {
            MyUtils.Log($"BottomBox is FULL");
            GlobalEventHandler.OnBottomBoxIsFull?.Invoke();
        }
    }
    private void ShowObjectMatchAnimation(List<BallEntity> matchedObjects, List<int> matchedObjectIndices)
    {
        int meanIndex = GetMeanIndex(matchedObjectIndices);
        foreach (BallEntity entity in matchedObjects)
        {
            TweenToBottomCollection(entity, _ballHolders[meanIndex].holder.position, 0.15f, () =>
            {
                entity.ShrinkAndDestroy();
                matchedObjects.Remove(entity);
                ShowRearrangeAnimation();
            });
        } 
        MyUtils.DelayedCallback(0.12f, () =>
        {
            if (!_ballHolders[meanIndex].ShowBlastEffect())
            {
                ShowBackupParticleEffect(meanIndex);
            }
            //DeftEventHandler.TriggerEvent(EventID.REQUEST_STARS_EFFECT, uiObjectHolders[meanIndex].starsParticleSystem);
        });
    }
    private void ShowBackupParticleEffect(int index)
    {
        _backupParticleEffect.transform.position = _ballHolders[index].holder.position;
        _backupParticleEffect.Stop();
        _backupParticleEffect.Play();
    }

    private int GetMeanIndex(List<int> indices)
    {
        int meanIndex = 0;
        foreach (int i in indices)
        {
            meanIndex += i;
        }
        return (meanIndex / 3);
    }
    private void RearrangeObjectsOnInsert(int startIndex = 1)
    {
        if (startIndex >= _collectedEntities.Count) return;
        if (_ballHolders[startIndex].isOccupied)
            RearrangeObjectsOnInsert(startIndex + 1);
        else
            return;
        AddObjectsToNewIndex(startIndex, startIndex + 1);
    }
    private void ShowRearrangeAnimation()
    {
        foreach (BallHolder entityHolder in _ballHolders)
            if (entityHolder.isOccupied)
                TweenToBottomCollection(entityHolder.objectEntity, entityHolder.holder.position, 0.1f);
    }
    private void AddObjectsToNewIndex(int oldIndex, int newIndex)
    {
        _ballHolders[newIndex].SetDataFromEntity(_ballHolders[oldIndex].objectEntity);
        _ballHolders[oldIndex].ResetHolder();
        Debug.Log($"!! Indices From : old: {oldIndex} new: {newIndex} name: {_ballHolders[newIndex].objectEntity.name}");
    }

    private void TweenToBottomCollection(BallEntity entity, Vector3 pose, float duration = 0.35f, System.Action onComplete = null)
    {
        entity.transform.DOMove(pose, duration).SetSpeedBased(false).SetUpdate(true).onComplete += () =>
        {
            onComplete?.Invoke();
        };
    }

    private int GetIndexForEntry(BallEntity entity)
    {
        int index = _collectedEntities.FindLastIndex(x => x.Id.Equals(entity.Id));//returns -1 if not found.
        if (index != -1) index += 1;
        else if (index == -1)
            index = _collectedEntities.Count;
        else if (index >= _collectedEntities.Count)
        {
            Debug.Log($"Collection is full: {_collectedEntities.Count}");
            index = -1;
        }
        return index;
    }

    private void Callback_On_Ball_Selected(BallEntity selectedEntity)
    {
        Debug.Log($"{selectedEntity.name} is selected...");

        if (_collectedEntities.Count >= _ballHolders.Count)
        {
            selectedEntity.Activate();
            return;
        }
        AddObjectToBottomCollection(selectedEntity);

    }

    [System.Serializable]
    public class BallHolder
    {
        public Transform holder;
        public ParticleSystem blastEffect;
        [HideInInspector]
        public BallEntity objectEntity;
        [HideInInspector]
        public bool isOccupied;
        [HideInInspector]
        public int id;

        public void SetDataFromEntity(BallEntity entity)
        {
            id = entity.Id;
            objectEntity = entity;
            isOccupied = true;
        }
        public void ResetHolder()
        {
            id = -1;
            objectEntity = null;
            isOccupied = false;
        }

        public bool ShowBlastEffect()
        {
            if (blastEffect.IsAlive()) return false;
            blastEffect.Play();
            return true;
        }
    }


}
