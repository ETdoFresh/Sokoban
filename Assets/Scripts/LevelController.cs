﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Planning;
using Planning.IO;
using StateSpaceSearchProject;
using HeuristicSearchPlanner;

public class LevelController : MonoBehaviour
{
    static public int MAX_LEVEL = 2;
    static public int CURRENT_LEVEL = 1;
    static public event Action OnMove = delegate { };
    static public TextAsset domain;
    static public int num_times_enables = 0;

    public int numberTimesEnables = 0;
    public MainGrid grid;
    public GameObject player;
    public Cell playerCell;
    public int boxStrength = 1;
    public GameObject SolidPrefab;
    public GameObject MoveablePrefab;
    public GameObject TargetPrefab;
    public GameObject PlayerPrefab;
    public GameObject CompleteMenuPrefab;
    public GameObject FailMenuPrefab;
    public GameObject PauseMenuPrefab;
    public GameObject StartingCinematicPrefab;
    public GameObject MiddleCinematicPrefab;
    public GameObject EndingCinematicPrefab;
    public Level level;
    private int _boxesPushed = 0;
    private bool _isComplete;
    private bool _isPause;

    enum Direction { UP, DOWN, LEFT, RIGHT }

    void OnEnable()
    {
        num_times_enables++;
        numberTimesEnables = num_times_enables;

        InputController.OnUp += MoveUp;
        InputController.OnDown += MoveDown;
        InputController.OnLeft += MoveLeft;
        InputController.OnRight += MoveRight;
        CompleteMenu.OnClickMenu += FadeOutAndDestroy;
        CompleteMenu.OnClickRestart += FadeOutAndDestroy;
        CompleteMenu.OnClickNextLevel += FadeOutAndDestroy;
        PauseMenu.OnClickResume += ResumeGame;
    }

    void CreateLevel()
    {
        level = Level.Create(LevelReader.ReadLevel("Assets/Level/level" + CURRENT_LEVEL + ".txt"));
        grid = GameObjectHelper.FindChildByName(gameObject, "Ground").GetComponent<MainGrid>();
        grid.transform.localScale = new Vector3(level.width, level.height, level.height);
        grid.SetGrid(level.width, level.height);
        Camera.main.transform.localPosition = new Vector3(0, 10, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(90, 0, 0);
    }

    void PlayCinematic(GameObject CinematicPrefab, bool resume = true)
    {
        if (CinematicPrefab == null)
            return;

        PauseGame();
        Instantiate(CinematicPrefab).name = CinematicPrefab.name;

        if (resume)
            Cinematic.OnCinematicFinish += ResumeGame;
    }

    void OnDisable()
    {
        InputController.OnUp -= MoveUp;
        InputController.OnDown -= MoveDown;
        InputController.OnLeft -= MoveLeft;
        InputController.OnRight -= MoveRight;
        CompleteMenu.OnClickMenu -= FadeOutAndDestroy;
        CompleteMenu.OnClickRestart -= FadeOutAndDestroy;
        CompleteMenu.OnClickNextLevel -= FadeOutAndDestroy;
        PauseMenu.OnClickResume -= ResumeGame;
    }

    void Start()
    {
        CreateLevel();
        GameObject solids = new GameObject() { name = "Solids" };
        GameObject moveables = new GameObject() { name = "Moveable Boxes" };
        GameObject targets = new GameObject() { name = "Targets" };

        solids.transform.SetParent(transform);
        moveables.transform.SetParent(transform);
        targets.transform.SetParent(transform);

        for (int x = 0; x < level.width; x++)
            for (int y = 0; y < level.height; y++)
                switch (level.data[x, y])
                {
                    case '1':
                        GameObject solid = Spawn(SolidPrefab, grid.GetCell(x, y));
                        solid.name += " " + solids.transform.childCount;
                        solid.transform.SetParent(solids.transform);
                        break;
                    case 'S':
                        player = Spawn(PlayerPrefab, grid.GetCell(x, y));
                        player.transform.SetParent(transform);
                        playerCell = player.GetComponent<Cell>();
                        break;
                    case 'B':
                        GameObject box = Spawn(MoveablePrefab, grid.GetCell(x, y));
                        box.name += " " + moveables.transform.childCount;
                        box.transform.SetParent(moveables.transform);
                        break;
                    case 'T':
                        GameObject target = Spawn(TargetPrefab, grid.GetCell(x, y));
                        target.name += " " + targets.transform.childCount;
                        target.transform.SetParent(targets.transform);
                        grid.GetCell(x, y).GetComponent<CellManager>().gameObjectOnMe = null;
                        break;
                }

        if (domain == null)
            domain = Resources.Load<TextAsset>("Sokoban_domain");

        LevelState ls = new LevelState(gameObject, level);
        string pddl = ls.ToPDDL();
        using (StreamWriter writer = new StreamWriter("Sokoban_problem.txt"))
            writer.Write(pddl);

        Problem problem = PDDLReader.GetProblem(domain.text, pddl);
        StateSpaceProblem ssProblem = new StateSpaceProblem(problem);
        HSPlanner hsp = new HSPlanner(ssProblem);
        Plan plan = hsp.findNextSolution();

        PlayCinematic(StartingCinematicPrefab);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !_isComplete && !_isPause)
        {
            _isComplete = true;
            Instantiate(FailMenuPrefab).name = FailMenuPrefab.name;
            PlayCinematic(EndingCinematicPrefab, false);
            PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.P) && !_isComplete && !_isPause)
        {
            Instantiate(PauseMenuPrefab).name = PauseMenuPrefab.name;
            PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.C) && !_isComplete && !_isPause)
        {
            PlayCinematic(MiddleCinematicPrefab);
        }
    }

    private void PauseGame()
    {
        _isPause = true;
        InputController.OnUp -= MoveUp;
        InputController.OnDown -= MoveDown;
        InputController.OnLeft -= MoveLeft;
        InputController.OnRight -= MoveRight;
    }

    private void ResumeGame()
    {
        Cinematic.OnCinematicFinish -= ResumeGame;
        _isPause = false;
        InputController.OnUp += MoveUp;
        InputController.OnDown += MoveDown;
        InputController.OnLeft += MoveLeft;
        InputController.OnRight += MoveRight;
    }

    void MoveUp() { Move(playerCell, Direction.UP); }
    void MoveDown() { Move(playerCell, Direction.DOWN); }
    void MoveLeft() { Move(playerCell, Direction.LEFT); }
    void MoveRight() { Move(playerCell, Direction.RIGHT); }

    void Move(Cell cell, Direction direction)
    {
        GameObject destinationCell = null;
        int x = cell.x;
        int y = cell.y;
        switch (direction)
        {
            case Direction.UP:
                destinationCell = grid.GetCell(x, y + 1);
                break;
            case Direction.DOWN:
                destinationCell = grid.GetCell(x, y - 1);
                break;
            case Direction.LEFT:
                destinationCell = grid.GetCell(x - 1, y);
                break;
            case Direction.RIGHT:
                destinationCell = grid.GetCell(x + 1, y);
                break;
        }

        if (isValidMove(destinationCell, direction))
        {
            cell.SetCell(destinationCell);
            if (cell.gameObject == player)
                OnMove();
        }

        _boxesPushed = 0;
        CheckTargets();
    }

    bool isValidMove(GameObject cell, Direction direction)
    {
        if (cell == null)
            return false;

        if (!isPassable(cell))
            return false;

        if (!isMovable(cell, direction))
            return false;

        return true;
    }

    private bool isMovable(GameObject cell, Direction direction)
    {
        CellManager cellManager = cell.GetComponent<CellManager>();
        if (cellManager.gameObjectOnMe != null)
        {
            Cell cellsGameObjectCell = cellManager.gameObjectOnMe.GetComponent<Cell>();
            _boxesPushed++;
            if (_boxesPushed <= boxStrength)
                Move(cellsGameObjectCell, direction);

            if (cellManager.gameObjectOnMe == null)
                return true;

            return false;
        }
        return true;
    }

    private bool isPassable(GameObject cell)
    {
        CellManager cellManager = cell.GetComponent<CellManager>();
        if (cellManager.gameObjectOnMe != null)
        {
            Cell cellsGameObjectCell = cellManager.gameObjectOnMe.GetComponent<Cell>();
            if (cellsGameObjectCell != null)
                return !cellsGameObjectCell.isSolid;
        }
        return true;
    }

    private void CheckTargets()
    {
        bool allTargetsDone = true;
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject target in targets)
        {
            Cell cell = target.GetComponent<Cell>();
            GameObject c = grid.grid[cell.x, cell.y];
            CellManager cm = c.GetComponent<CellManager>();
            GameObject onCell = cm.gameObjectOnMe;
            if (onCell == null || onCell.name == "Player")
            {
                allTargetsDone = false;
                break;
            }
        }

        if (targets.GetLength(0) == 0)
            return;

        if (allTargetsDone && !_isComplete)
        {
            PlayCinematic(EndingCinematicPrefab, false);
            PauseGame();
            _isComplete = true;
            Instantiate(CompleteMenuPrefab).name = CompleteMenuPrefab.name;
        }
    }

    GameObject Spawn(GameObject spawnObject, GameObject cell)
    {
        GameObject newObject = Instantiate(spawnObject);
        newObject.name = spawnObject.name;
        newObject.GetComponent<Cell>().SetCell(cell);
        return newObject;
    }

    void FadeOutAndDestroy()
    {
        Destroy(gameObject);
    }
}
