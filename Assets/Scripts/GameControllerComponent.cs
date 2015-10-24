using System;
using UnityEngine;

public class GameControllerComponent : MonoBehaviour
{
    public GameObject logo;
    public GameObject menu;
    public GameObject game;

    void OnEnable()
    {
        GameController.component = this;

        Logo.OnLogoFinish += StartMenu;
        Menu.OnClickStart += StartGame;
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
        Instantiate(logo).name = logo.name;        
    }

    void StartMenu()
    {
        Instantiate(menu).name = menu.name;
    }

    void StartGame()
    {
        Instantiate(game).name = game.name;
    }

    void StartGameNextLevel()
    {
        LevelController.CURRENT_LEVEL = 
            Math.Min(LevelController.CURRENT_LEVEL + 1, LevelController.MAX_LEVEL);
        StartGame();
    }
}
