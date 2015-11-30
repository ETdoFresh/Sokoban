using UnityEngine;
using System.Collections;
using UnityThread;
using StateSpaceSearchProject;
using HeuristicSearchPlanner;
using System.Collections.Generic;

public class PlannerThread : ThreadJob
{
    private StateSpaceProblem _ssProblem;
    private StateSpaceSearchET _planner;
    private Dictionary<StateSpaceNode, int> _result;

    public PlannerThread(StateSpaceProblem ssProblem, string plannerType, bool useNovelty)
    {
        _ssProblem = ssProblem;

        switch (plannerType)
        {
            case "BFS":
                //_planner = new BFSPlanner(_ssProblem);
                break;
            case "HSP":
                _planner = new HSPlanner(_ssProblem);
                break;
            case "FF":
                //_planner = new FFPlanner(_ssProblem);
                break;
        }
    }

    protected override void ThreadFunction()
    {
        _result = _planner.GetNextStates();
        base.ThreadFunction();
    }

    public Dictionary<StateSpaceNode, int> GetResult()
    {
        return _result;
    }
}
