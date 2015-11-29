using UnityEngine;
using System.Collections;
using UnityThread;
using StateSpaceSearchProject;
using HeuristicSearchPlanner;
using System.Collections.Generic;

public class HSPThread : ThreadJob
{
    private StateSpaceProblem _ssProblem;
    private HSPlanner _hsp;
    private Dictionary<StateSpaceNode, int> _result;

    public HSPThread(StateSpaceProblem ssProblem)
    {
        _ssProblem = ssProblem;
        _hsp = new HSPlanner(_ssProblem);
    }

    protected override void ThreadFunction()
    {
        _result = _hsp.GetNextStatesCosts(1);
        base.ThreadFunction();
    }

    public Dictionary<StateSpaceNode, int> GetResult()
    {
        return _result;
    }
}
