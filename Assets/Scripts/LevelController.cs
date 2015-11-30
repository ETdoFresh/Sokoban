﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Planning;
using Planning.IO;
using StateSpaceSearchProject;
using HeuristicSearchPlanner;
using Planning.Logic;
using System.Collections.Generic;
using UnityThread;

public class LevelController : MonoBehaviour
{
    static public int MAX_LEVEL = 4;
    static public int CURRENT_LEVEL = 1;
    static public event Action OnMove = delegate { };
    static public TextAsset domain;
    static public int num_times_enables = 0;
    static readonly Vector2 NullVector2 = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

    public int numberTimesEnables = 0;
    public MainGrid grid;
    public GameObject player;
    public Cell playerCell;
    public int boxStrength = 1;
    public GameObject SolidPrefab;
    public GameObject MoveablePrefab;
    public GameObject TargetPrefab;
    public GameObject PlayerPrefab;
    public GameObject PlayerGhostPrefab;
    public GameObject BoxGhostPrefab;
    public GameObject PaperPrefab;
    public GameObject CompleteMenuPrefab;
    public GameObject FailMenuPrefab;
    public GameObject PauseMenuPrefab;
    public GameObject PlanMenuPrefab;
    public GameObject StartingCinematicPrefab;
    public GameObject MiddleCinematicPrefab;
    public GameObject EndingCinematicPrefab;
    public Level level;
    private int _boxesPushed = 0;
    private bool _isComplete;
    private bool _isPause;
    private StateSpaceProblem _ssProblem;
    private PlannerThread _hThread;
    private bool _isLookingAtPaper = false;
    private GameObject ghosts;
    private string _plannerType = null;
    private bool _useNovelty;

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
        char[,] data = LevelReader.ReadLevel(CURRENT_LEVEL);
        level = Level.Create(data);
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

        Status.SetText("Playing Cinematic...");
        PauseGame();
        GameObject cinematic = Instantiate(CinematicPrefab);
        cinematic.name = CinematicPrefab.name;
        cinematic.transform.SetParent(transform);

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

        Status.SetText("Showing Level " + CURRENT_LEVEL + "...");
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
        CheckForHThread();
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
        DeleteGhosts();

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

    private void DeleteGhosts()
    {
        if (ghosts != null)
            Destroy(ghosts);
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

    public void ClickPlan()
    {
        _isLookingAtPaper = true;
        _ssProblem = null;
        _plannerType = null;
        DeleteGhosts();

        GameObject planMenu = Instantiate(PlanMenuPrefab);
        planMenu.transform.SetParent(transform);
        PlanMenuController.OnClickCompute += ShowDomainPDDL;
        PlanMenuController.OnClickCancel += FinishWithPaper;

        GetProblem();
    }

    private void GetProblem()
    {
        LevelState ls = new LevelState(gameObject, level);
        string pddl = ls.ToPDDL();
        Debug.Log("Thread Started - Getting Problem...");
        ThreadJob thread = new ProblemThread(pddl, level, domain.text);
        thread.Start();
        thread.OnThreadComplete += ProblemThreadCallback;
        thread.OnThreadAbort += Abort;
    }

    private void ShowDomainPDDL(PlanMenuController planMenu)
    {
        PlanMenuController.OnClickCompute -= ShowDomainPDDL;
        PauseGame();

        _plannerType = planMenu.GetPlanner();
        _useNovelty = planMenu.UseNovelty();

        GameObject paper = Instantiate(PaperPrefab);
        paper.transform.localPosition += new Vector3(0, 3, 0);
        paper.transform.FindChild("Paper Object").localEulerAngles -= new Vector3(90, 0, 0);
        paper.transform.FindChild("Paper Object").localScale = Vector3.one * 5;
        paper.transform.FindChild("Paper Object").localPosition += new Vector3(0, 0, -10);
        paper.GetComponentInChildren<PaperResize>().SetText(domain.text);
        PaperController.OnClickDone += ShowProblemPDDL;
        Status.SetText("Showing Domain PDDL...");

        if (IsReadyToSolve())
            GetNextState();
    }

    private void ShowProblemPDDL()
    {
        PaperController.OnClickDone -= ShowProblemPDDL;
        LevelState ls = new LevelState(gameObject, level);
        string pddl = ls.ToPDDL();

        GameObject paper = Instantiate(PaperPrefab);
        paper.transform.localPosition += new Vector3(0, 3, 0);
        paper.transform.FindChild("Paper Object").localEulerAngles -= new Vector3(90, 0, 0);
        paper.transform.FindChild("Paper Object").localScale = Vector3.one * 5;
        paper.transform.FindChild("Paper Object").localPosition += new Vector3(0, 0, -10);
        paper.GetComponentInChildren<PaperResize>().SetText(pddl);
        PaperController.OnClickDone += FinishWithPaper;
        Status.SetText("Showing Problem PDDL...");
    }

    private void FinishWithPaper()
    {
        PaperController.OnClickDone -= FinishWithPaper;
        PlanMenuController.OnClickCancel -= FinishWithPaper;
        _isLookingAtPaper = false;

        if (_hThread == null)
            Status.SetText("Still Computing...");
    }

    private void ProblemThreadCallback(ThreadJob thread)
    {
        Debug.Log("Thread Complete - Getting Problem...");
        if (thread is ProblemThread)
        {
            ProblemThread pThread = (ProblemThread)thread;
            _ssProblem = pThread.GetStateSpaceProblem();
            Debug.Log(_ssProblem);
        }
        thread.ResetEventSubscriptions();

        if (IsReadyToSolve())
            GetNextState();
    }

    private bool IsReadyToSolve()
    {
        return _ssProblem != null && _plannerType != null;
    }

    private void GetNextState()
    {
        Debug.Log("Thread Started - Getting Next States...");
        ThreadJob thread = new PlannerThread(_ssProblem, _plannerType, _useNovelty);
        thread.Start();
        thread.OnThreadComplete += PlannerCallback;
        thread.OnThreadAbort += Abort;
    }

    private void Abort(ThreadJob thread)
    {
        Debug.Log("Thread has been aborted.");
        thread.ResetEventSubscriptions();
    }

    private void PlannerCallback(ThreadJob thread)
    {
        Debug.Log("Thread Complete - Getting Next States...");
        _hThread = (PlannerThread)thread;
        thread.ResetEventSubscriptions();
    }

    private void CheckForHThread()
    {
        if (_hThread == null || _isLookingAtPaper)
            return;

        if (_isPause) ResumeGame();
        ghosts = new GameObject();
        ghosts.name = "Ghosts";
        ghosts.transform.SetParent(transform);
        foreach (KeyValuePair<StateSpaceNode, int> entry in _hThread.GetResult())
        {
            Vector2 playerPosition = GetPlayerPosition(entry.Key.state);
            int x = Convert.ToInt32(playerPosition.x);
            int y = Convert.ToInt32(playerPosition.y);
            if (grid.grid[x, y].GetComponent<CellManager>().gameObjectOnMe == null)
            {
                GameObject playerGhost = Instantiate(PlayerGhostPrefab);
                playerGhost.transform.SetParent(ghosts.transform);
                playerGhost.GetComponent<Cell>().SetCell(grid.grid[x, y]);
                string cost = entry.Value == int.MaxValue ? "∞" : entry.Value.ToString();
                playerGhost.transform.FindChild("Cost").GetComponent<TextMesh>().text = cost;
                grid.grid[x, y].GetComponent<CellManager>().gameObjectOnMe = null;
            }

            foreach (Vector2 boxPosition in GetBoxPositions(entry.Key))
            {
                x = Convert.ToInt32(boxPosition.x);
                y = Convert.ToInt32(boxPosition.y);
                if (grid.grid[x, y].GetComponent<CellManager>().gameObjectOnMe == null)
                {
                    GameObject boxGhost = Instantiate(BoxGhostPrefab);
                    boxGhost.transform.SetParent(ghosts.transform);
                    boxGhost.GetComponent<Cell>().SetCell(grid.grid[x, y]);
                    string cost = entry.Value == int.MaxValue ? "∞" : entry.Value.ToString();
                    boxGhost.transform.FindChild("Cost").GetComponent<TextMesh>().text = cost;
                    grid.grid[x, y].GetComponent<CellManager>().gameObjectOnMe = null;
                }
            }
        }
        Status.SetText("Next States Shown...");
        _hThread = null;
    }

    private List<Vector2> GetBoxPositions(StateSpaceNode node)
    {
        List<Vector2> boxPositions = new List<Vector2>();
        foreach (Step step in node.plan)
            if (!step.name.Contains("pushbox"))
                return boxPositions;

        State state = node.state;
        foreach (Literal literal in state.Literals)
            if (literal is Predication)
            {
                Predication predication = (Predication)literal;
                if (predication.predicate == "has_box")
                {
                    string[] cell = predication.terms.get(0).name.Split('_');
                    boxPositions.Add(new Vector2(Convert.ToSingle(cell[1]), Convert.ToSingle(cell[2])));
                }
            }
        return boxPositions;
    }

    private Vector2 GetPlayerPosition(State state)
    {
        foreach (Literal literal in state.Literals)
            if (literal is Predication)
            {
                Predication predication = (Predication)literal;
                if (predication.predicate == "has_player")
                {
                    string[] cell = predication.terms.get(0).name.Split('_');
                    return new Vector2(Convert.ToSingle(cell[1]), Convert.ToSingle(cell[2]));
                }
            }
        return NullVector2;
    }
}
