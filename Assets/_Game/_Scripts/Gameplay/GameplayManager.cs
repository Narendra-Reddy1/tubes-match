using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private List<TubeHandler> _tubeHandlers;
    [SerializeField] private List<ParticleSystem> _confettiParticlesList;
    private void OnEnable()
    {
        GlobalEventHandler.RequestToCheckAllBallsCleared += Callback_On_Ball_Entity_Selected;
    }
    private void OnDisable()
    {
        GlobalEventHandler.RequestToCheckAllBallsCleared -= Callback_On_Ball_Entity_Selected;
    }
    private void _ShowConfetti()
    {
        _confettiParticlesList.ForEach(x => x.Play());
    }
    private void Callback_On_Ball_Entity_Selected()
    {
        bool isCleared = _tubeHandlers.Any(x => !x.IsTubeEmpty());//caching to a variable just for debugging purpose;
        if (isCleared) return;
        _ShowConfetti();
        GlobalEventHandler.OnAllBallsCleared?.Invoke();

    }
}
