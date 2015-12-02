using BreadthFirstSearch;
using GraphPlanSGW;
using HeuristicSearchPlanner;
using PlanGraphSGW;
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
            PlanGraphPlanner pgPlanner = new GraphPlan();
            PlanGraphSearch pgSearch = new PlanGraphSearch(pgPlanner, problem);
            pgSearch.findNextSolution();
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
