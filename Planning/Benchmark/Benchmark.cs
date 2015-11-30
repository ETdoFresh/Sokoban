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
            string problemFile = "Sokoban_Problem_1.txt";
            string domainString = ReadFile(domainFile);
            string problemString = ReadFile(problemFile);
            Problem problem = PDDLReader.GetProblem(domainString, problemString);
            StateSpaceProblem ssProblem = new StateSpaceProblem(problem);
            HSPlanner hsp = new HSPlanner(ssProblem);
            Plan plan = hsp.findNextSolution();
        }

        private static string ReadFile(string filename)
        {
            using (StreamReader streamReader = new StreamReader(filename))
                return streamReader.ReadToEnd();
        }
    }
}
