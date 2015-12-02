using BreadthFirstSearch;
using GraphPlanProject;
using HeuristicSearchPlanner;
using PlanGraphProject;
using Planning;
using Planning.IO;
using StateSpaceSearchProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Benchmark
{
    class Benchmark
    {
        static void Main(string[] args)
        {
            string domainFile = "Sokoban_domain.txt";
            string problemFile = "Sokoban_Problem_0.txt";
            string domainString = ReadFile(domainFile);
            string problemString = ReadFile(problemFile);
            Problem problem = PDDLReader.GetProblem(domainString, problemString);
            PlanGraph planGraph = new PlanGraph(problem);
            GraphPlanNoYield graphPlan = new GraphPlanNoYield(planGraph, problem);
            graphPlan.FindPlan();
            //HSPlanner hsp = new HSPlanner(ssProblem);
            //Plan plan = hsp.findNextSolution();
        }

        private static string ReadFile(string filename)
        {
            using (StreamReader streamReader = new StreamReader(filename))
                return streamReader.ReadToEnd();
        }
    }
}
