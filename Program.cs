using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace VizzHuiz
{

    class Program
    {
        static ConsoleDrawer Drawer;
        static Random Random = new Random();
        static void DrawSerpinsky(Vector2 p1, Vector2 p2, Vector2 p3, int step, int maxSteps)
        {
            if (step == maxSteps)
            {
                return;
            }

            Drawer.DrawTriangle(p1, p2, p3);
            var hv12 = (p2 + p1) / 2;
            var hv13 = (p3 + p1) / 2;
            var hv23 = (p3 + p2) / 2;

            DrawSerpinsky(  p1, hv12, hv13, step + 1, maxSteps);
            DrawSerpinsky(hv12, hv13, hv23, step + 1, maxSteps);
            DrawSerpinsky(hv13, hv23, p3, step + 1, maxSteps);
            DrawSerpinsky(hv12, p2, hv23, step + 1, maxSteps);
        }
        static ConcurrentDictionary<string, Graph> Graphs;

        public static string HELP =>
        "graph [GRAPH_NAME] [NODES_COUNT]\n" +
        "tree [GRAPH_NAME] [NODES_COUNT]\n" +
        "draw [GRAPH | BFS | DFS | RADIUS] [GRAPH_NAME] [START_NODE] - optional(default:first) [DELAY] - optional(default:0)\n" +
        "replane [GRAPH_NAME] [STACKING TYPE] - circled/random\n" +
        "drop [GRAPH_NAME]\n" +
        "dropall\n" +
        "You don't a baby, can figure it out yourself...";
        static void ExecuteCommandsAsync()
        {
            Console.WriteLine("Type help to give more information.");
            while (true)
            {
                Console.Write("VizzHuiz# ");

                var commands = Console.ReadLine().Split(' ')
                .Where(c => c!= "")
                .Select(c => c.ToLower())
                .ToArray();

                var options = commands
                .Where(c => c.StartsWith('-') && c != "-")
                .ToArray();

                switch (commands[0])
                {
                    case "graph":
                        CreateGraph(commands, options);
                        break;

                    case "tree":
                        CreateGraph(commands, options, true);
                        break;

                    case "end":
                        Console.WriteLine("Goodbye motherfucker");
                        return;

                    case "draw":
                        ExecuteDrawings(commands, options);
                        break;
                    
                    case "help":
                        Console.WriteLine(HELP);
                        break;

                    case "replane":
                        ReplaneGraph(commands, options);
                        break;

                    case "list":
                        foreach (var key_val in Graphs)
                        {   
                            Console.WriteLine($"graph {key_val.Key} with {key_val.Value.NodesCount} nodes");
                        }
                        break;
                    
                    case "dropall":
                        Graphs.Clear();
                        break;

                    default:
                        Console.WriteLine($"Unknown command {commands[0]}");
                        break;
                }
            }
        }

        static void ReplaneGraph(string[] commands, string[] options)
        {
            Graph graph;
            if (!Graphs.TryGetValue(commands[1], out graph))
            {
                Console.WriteLine($"Graph {commands[1]} not found");
                return;
            }

            switch(commands[2])
            {
                case "circled":
                    graph.ReplaneGraphCircled();
                    break;
                case "random":
                    graph.ReplaneGraphRandom();
                    break;
            }

            Console.WriteLine($"Successfully replaned graph {commands[1]}");
        }
        static void CreateGraph(string[] commands, string[] options, bool tree = false)
        {
            int size;
            if (!int.TryParse(commands[2], out size))
            {
                Console.WriteLine($"Incorrect size {commands[2]}");
            }

            var graph = tree ? Graph.CreateRandomTree(size) : Graph.CreateRandom(size);
            Graphs[commands[1]] = graph;

            Console.WriteLine($"Successfully created {commands[0]} with {size} nodes");
        }

        static void ExecuteDrawings(string[] commands, string[] options)
        {
            var graph = Graphs[commands[2]];

            int delay;
            Node startNode;

            if(commands[1] == "graph")
            {
                graph.DrawGraphToConsole();
            }
            else    
            {
                startNode = commands.Length < 4 ? graph.Nodes.First() 
                    : graph.Nodes.First(n => n.Value == commands[3]);

                delay = commands.Length < 5 ? 0 : int.Parse(commands[4]);

                switch(commands[1])
                {
                    case "bfs":
                        graph.DrawBFSToConsole(startNode, delay);
                        break;
                    case "dfs":
                        graph.DrawDFSToConsole(startNode, delay);
                        break;
                }
            }
        }
        static void Main(string[] args)
        {
            var graph = Graph.CreateRandomTree(30);
            graph.ReplaneGraphCircled();
            graph.DrawBFSToConsole(graph.CenterNode, 10);

            // Graphs = new ConcurrentDictionary<string, Graph>();
            //  var execute = Task.Run(() => ExecuteCommandsAsync());
            //  while(!execute.IsCompleted);
        }
    }
}
