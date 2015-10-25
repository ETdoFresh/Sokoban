using System;
using UnityEngine;

public class GameControllerComponent : MonoBehaviour
{
    public GameObject logoPrefab;
    public GameObject menuPrefab;
    public GameObject levelSelectPrefab;
    public GameObject levelPrefab;

    void OnEnable()
    {
        GameController.component = this;

        Logo.OnLogoFinish += StartMenu;
        Menu.OnClickStart += StartLevelSelect ;
        LevelSelect.OnClickLevel += StartGameAtLevel;
        CompleteMenu.OnClickMenu += StartMenu;
        CompleteMenu.OnClickRestart += StartGame;
        CompleteMenu.OnClickNextLevel += StartGameNextLevel;
    }

    void OnDisable()
    {
        //GameController.component = null;
        Logo.OnLogoFinish -= StartMenu;
        Menu.OnClickStart -= StartGame;
        CompleteMenu.OnClickMenu -= StartMenu;
        CompleteMenu.OnClickRestart -= StartGame;
        CompleteMenu.OnClickNextLevel -= StartGameNextLevel;
    }
    
    void Start()
    {
        StartLogo();
    }

    void StartLogo()
    {
        Instantiate(logoPrefab).name = logoPrefab.name;        
    }

    void StartMenu()
    {
        Instantiate(menuPrefab).name = menuPrefab.name;
    }

    void StartLevelSelect()
    {
        Instantiate(levelSelectPrefab).name = levelSelectPrefab.name;
    }

    void StartGame()
    {
        Instantiate(levelPrefab).name = levelPrefab.name;
    }

    void StartGameNextLevel()
    {
        LevelController.CURRENT_LEVEL = 
            Math.Min(LevelController.CURRENT_LEVEL + 1, LevelController.MAX_LEVEL);
        StartGame();
    }

    void StartGameAtLevel(int level)
    {
        LevelController.CURRENT_LEVEL = Math.Min(level, LevelController.MAX_LEVEL);
        StartGame();
    }
}
