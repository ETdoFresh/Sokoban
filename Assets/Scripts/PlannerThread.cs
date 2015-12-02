using UnityEngine;
using System.Collections;
using UnityThread;
using StateSpaceSearchProject;
using HeuristicSearchPlanner;
using System.Collections.Generic;
using System;
using Planning;

public class PlannerThread : ThreadJob
{
    private StateSpaceProblem _ssProblem;
    private StateSpaceSearchET _planner;
    private Dictionary<StateSpaceNode, int> _result;
    private Plan _plan;
    private string _plannerFunction;

    public PlannerThread(StateSpaceProblem ssProblem, string plannerFunction, string plannerType, bool useNovelty)
    {
        _ssProblem = ssProblem;
        _plannerFunction = plannerFunction;

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
        if (_plannerFunction == "NextState")
            _result = _planner.GetNextStates();
        else
            _plan = _planner.findNextSolution();
        base.ThreadFunction();
    }

    public Dictionary<StateSpaceNode, int> GetResult()
    {
        return _result;
    }

    public Plan GetPlan()
    {
        return _plan;
    }

    public StateSpaceNode GetCurrentNode()
    {
        return _planner.GetCurrentNode();
    }

    private Level ConvertToLevel(StateSpaceNode node)
    {
        throw new NotImplementedException();
    }
}
