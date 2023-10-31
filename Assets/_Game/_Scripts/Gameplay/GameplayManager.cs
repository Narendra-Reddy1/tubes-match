using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private List<TubeHandler> _tubeHandlers;

    private void OnEnable()
    {
        GlobalEventHandler.RequestToCheckAllBallsCleared += Callback_On_Ball_Entity_Selected;
    }
    private void OnDisable()
    {
        GlobalEventHandler.RequestToCheckAllBallsCleared-= Callback_On_Ball_Entity_Selected;
    }

    private void Callback_On_Ball_Entity_Selected()
    {
        bool isCleared = _tubeHandlers.Any(x => !x.IsTubeEmpty());
        Debug.Log($"CHECK BOARD CLEARED:::{isCleared}");
        if (isCleared) return;
        Debug.Log($"{isCleared}");
        GlobalEventHandler.OnAllBallsCleared?.Invoke();

    }
}
