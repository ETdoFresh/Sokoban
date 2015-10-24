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
    }

    void OnDisable()
    {
        //GameController.component = null;
        Logo.OnLogoFinish -= StartMenu;
        Menu.OnClickStart -= StartGame;
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
        Logo.OnLogoFinish -= StartMenu;
        Instantiate(menu).name = menu.name;
    }

    void StartGame()
    {
        Menu.OnClickStart -= StartGame;
        Instantiate(game).name = game.name;
    }
}
