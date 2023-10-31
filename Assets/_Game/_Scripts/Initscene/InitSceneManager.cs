using BenStudios.ScreenManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneManager : MonoBehaviour
{
    private void Start()
    {

        AsyncOperation handle = SceneManager.LoadSceneAsync(Konstants.PERSISTENT_MANAGERS, LoadSceneMode.Additive);
        Input.multiTouchEnabled = false;
        handle.completed += (op) =>
        {
            SceneManager.LoadSceneAsync(Konstants.MAIN_SCENE, LoadSceneMode.Additive).completed += (op1) =>
              {
                  ScreenManager.Instance.ChangeScreen(Window.GameplayScreen, isUIObject: false);
                  SceneManager.UnloadSceneAsync(Konstants.INIT_SCENE);
                  SceneManager.SetActiveScene(SceneManager.GetSceneByName(Konstants.MAIN_SCENE));
              };
        };



    }
}
