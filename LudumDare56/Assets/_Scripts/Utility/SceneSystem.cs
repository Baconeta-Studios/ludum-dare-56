using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    private enum Scenes
    {
        MainMenu = 0,
        Game = 1,
    }
    
    public void LoadScene(int sceneIndex)
    {
        
        Debug.Log($"Loading Scene {sceneIndex}: {(Scenes)sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }
}
 