using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Numerics;

namespace VizzHuiz
{
    class Node
    {
        public Vector2 Coords {get; set;}
        Dictionary<Node, List<Edge>> node_edges;
        public int AdjasentCount { get; private set; }
        public int EdgesCount { get; private set; }
        public IEnumerable<Node> AdjasentNodes => node_edges.Select(n_e => n_e.Key);
        public IEnumerable<Edge> IncidentEdges => node_edges.SelectMany(n_e => n_e.Value);
        public string Value {get;}
        public Node(string value, Vector2 coords)
        {
            Coords = coords;
            Value = value;
            node_edges = new Dictionary<Node, List<Edge>>();
        }
        public List<Edge> GetIncidentEdges(Node node)
        {
            List<Edge> edges;
            if(!node_edges.TryGetValue(node, out edges)) 
                return null;
            
            return edges;
        }
        public void Connect(Node node, int weight=1)
        {
            EdgesCount++;
            List<Edge> edges;
            if(!node_edges.TryGetValue(node, out edges))
                edges = new List<Edge>() {new Edge(this, node, weight)};
                node_edges[node] = edges;
                AdjasentCount++;

            node_edges[node].Add(new Edge(this, node, weight));
        }
    }
    class Edge
    {
        public double Weight {get;}
        public Node Node1 {get;}
        public Node Node2 {get;}
        public Edge(Node node1, Node node2, double weight =1)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
        }
    }
    class Path
    {
        public double TotalWeight { get; }
        public int VerticesCount { get; }
        public int EdgesCount => VerticesCount - 1;
        public Path Previous { get; }
        public Edge Edge { get; }
        public Path(Edge edge, Path prev)
        {
            if(prev != null && edge.Node1 != prev.Edge.Node2)
                throw new ArgumentException("Incorrect path data");

            Edge = edge;
            Previous = prev;

            if(prev == null)
            {
                TotalWeight = Edge.Weight;
                VerticesCount = 1;
            }
            else
            {
                TotalWeight = prev.TotalWeight + edge.Weight;
                VerticesCount = prev.VerticesCount + 1;
            }
        }
        public List<Node> ToList()
        {
            var vertices = new List<Node>(VerticesCount);
            var current = this;

            for (int i = VerticesCount - 1; i >= 0; i++)
            {
                vertices[i] = current.Edge.Node2;
                current = current.Previous;
            }

            return vertices;
        }
        public IEnumerable<Node> Nodes()
        {
            var current = this;
            while(current != null)
            {
                yield return current.Edge.Node2;
                current = current.Previous;
            }
        }
        public IEnumerable<Edge> Edges()
        {
            var current = this;
            while (current != null)
            {
                yield return current.Edge;
                current = current.Previous;
            }
        }
    }
    class Graph
    {
        HashSet<Node> node_cache;
        List<Node> node_list;
        public int Radius => RadiusPath.EdgesCount;
        public int Diameter { get; private set; }
        public Path RadiusPath { get; private set; }
        public Node CenterNode { get; private set; }
        public IEnumerable<Node> Nodes => node_list.Select(n => n);
        public IEnumerable<Edge> Edges => Nodes.SelectMany(n => n.IncidentEdges);
        public int NodesCount => node_list.Count;
        public void UpdateRadiusAndDiameter()
        {
            int mine = int.MaxValue;
            Diameter = 0;

            foreach (var paths in node_list.Select(n => BFSPaths(n)))
            {
                Path exentrisitet = null;
                foreach (var path in paths)
                {
                    int e = 0;
                    if (path.EdgesCount > e)
                    {
                        e = path.EdgesCount;
                        exentrisitet = path;
                    }
                }

                if(exentrisitet != null && exentrisitet.EdgesCount > Diameter) 
                    Diameter = exentrisitet.EdgesCount;

                if (exentrisitet != null && exentrisitet.EdgesCount < mine)
                {
                    RadiusPath = exentrisitet;
                    mine = exentrisitet.EdgesCount;
                }
            }

            var nodes = RadiusPath.Nodes().ToArray();
            CenterNode = nodes[0];
        }
        public Graph()
        {
            node_list = new List<Node>();
            node_cache = new HashSet<Node>();
        }
        public Graph(List<Node> node_list)
        {
            this.node_list = node_list;
            node_cache = node_list.ToHashSet();
            UpdateRadiusAndDiameter();
        }
        public static Graph CreateRandom(int size, bool oriedted = false)
        {
            Random rand = new Random();
            int x,y;

            //Creating node set
            var node_list = new List<Node>(size);
            for (int i = 0; i < size; i++)
            {
                x = rand.Next(0, Constants.InitialWidth);
                y = rand.Next(0, Constants.InitialHeight);

                var node = new Node($"{i}", new Vector2(x, y));
                node_list.Add(node);
            }

            //Connecting with each other
            foreach(var node in node_list)
            {
                int adjCount = rand.Next(1, size - 1);
                for (int i = 0; i < adjCount; i++)
                {
                    var adjNodeIndex = rand.Next(0, size - 1);
                    var adjNode = node_list[adjNodeIndex];
                    node.Connect(adjNode);
                    if(!oriedted) adjNode.Connect(node);
                }
            }

            return new Graph(node_list);
        }
        public static Graph CreateRandomTree(int size, bool oriented = false)
        {
            int x, y, total = size;
            var rand = new Random();

            var node_list = new List<Node>();
            var queue = new Queue<Node>();

            x = rand.Next(0, Constants.InitialWidth);
            y = rand.Next(0, Constants.InitialHeight);

            var first = new Node("0", new Vector2(x, y));

            queue.Enqueue(first);
            int k = 1;

            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                node_list.Add(current);

                int count = Math.Min(rand.Next(1, total / 3), size);
                for (int i = 0; i < count; i++)
                {
                    x = rand.Next(0, Constants.InitialWidth);
                    y = rand.Next(0, Constants.InitialHeight);
                    var adjNode = new Node($"{k}", new Vector2(x, y));
                    k++;
                    current.Connect(adjNode);
                    if(!oriented) adjNode.Connect(current);
                    queue.Enqueue(adjNode);
                }

                size -= count;
            }
            return new Graph(node_list);
        }
        public void AddNode(Node node)
        {
            node_list.Add(node);
            UpdateRadiusAndDiameter();
        }
        public void Connect(Node node1, Node node2)
        {
            if (!node_cache.Contains(node1))
            {
                throw new ArgumentException($"Node {node1.Value} not in graph");
            }

            if (!node_cache.Contains(node2))
            {
                throw new ArgumentException($"Node {node2.Value} not in graph");
            }

            node1.Connect(node2);
        }
        public IEnumerable<Edge> DFS(Node from)
        {
            if(!node_cache.Contains(from))
                throw new ArgumentException($"Node {from.Value} not in graph");
            return DFSHelper(from).Skip(1);
        }
        public IEnumerable<Edge> BFS(Node from)
        {
            if(!node_cache.Contains(from))
                throw new ArgumentException($"Node {from.Value} not in graph");
            return BFSHelper(from).Skip(1);
        }
        private IEnumerable<Edge> DFSHelper(Node from)
        {
            var visited = new HashSet<Node>();
            var stack = new Stack<Edge>();

            stack.Push(new Edge(null, from));

            while (stack.Count != 0)
            {
                var current = stack.Pop();

                if(visited.Contains(current.Node2)) continue;
                visited.Add(current.Node2);
                yield return current;

                foreach(var inc in current.Node2.IncidentEdges)
                {
                    stack.Push(inc);
                }
            }
        }
        private IEnumerable<Edge> BFSHelper(Node from)
        {
            var visited = new HashSet<Node>();
            var queue = new Queue<Edge>();

            queue.Enqueue(new Edge(null, from));

            while (queue.Count != 0)
            {
                var current = queue.Dequeue();

                if (visited.Contains(current.Node2)) continue;
                visited.Add(current.Node2);
                yield return current;

                foreach(var inc in current.Node2.IncidentEdges)
                {
                    queue.Enqueue(inc);
                }
            }
        }
        public IEnumerable<Path> BFSPaths(Node from)
        {
            var visited = new HashSet<Node>();
            var queue = new Queue<Path>();

            queue.Enqueue(new Path(new Edge(null, from), null));

            while(queue.Count != 0)
            {
                var current = queue.Dequeue();
                if(visited.Contains(current.Edge.Node2)) continue;
                visited.Add(current.Edge.Node2);

                yield return current;

                foreach (var inc in current.Edge.Node2.IncidentEdges)
                {
                    queue.Enqueue(new Path(inc, current));
                }
            }
        }
        public void DrawGraphToConsole()
        {
            var drawer = new ConsoleDrawer();
            GraphDrawHelper(drawer);
            drawer.ResetCursorTop();
        }
        public void DrawBFSToConsole(Node from, int delay)
        {
            var drawer = new ConsoleDrawer();
            GraphDrawHelper(drawer);
            drawer.SleepindAfterDraw = delay;
            BFSDrawHelper(from, drawer);
            drawer.ResetCursorTop();
        }
        public void DrawDFSToConsole(Node from, int delay)
        {
            var drawer = new ConsoleDrawer();
            GraphDrawHelper(drawer);
            drawer.SleepindAfterDraw = delay;
            DFSDrawHelper(from, drawer);
            drawer.ResetCursorTop();
        }
        public void DrawRadiusPathToConsole(int delay)
        {
            var drawer = new ConsoleDrawer();
            GraphDrawHelper(drawer);
            drawer.SleepindAfterDraw = delay;
            RadiusDrawHelper(drawer);
            drawer.ResetCursorTop();
        }
        private void GraphDrawHelper(ConsoleDrawer drawer)
        {
            DrawEdgesHelper(drawer, Edges, ConsoleColor.White);
        }
        private void BFSDrawHelper(Node from, ConsoleDrawer drawer)
        {
            DrawEdgesHelper(drawer, BFS(from), ConsoleColor.Red);
        }
        private void DFSDrawHelper(Node from, ConsoleDrawer drawer)
        {
            DrawEdgesHelper(drawer, DFS(from), ConsoleColor.Red);
        }
        private void RadiusDrawHelper(ConsoleDrawer drawer)
        {
            DrawEdgesHelper(drawer, RadiusPath.Edges(), ConsoleColor.Red);
        }
        private void DrawEdgesHelper(ConsoleDrawer drawer, IEnumerable<Edge> edges, ConsoleColor color)
        {
            foreach (var edge in edges)
            {
                //drawing edge
                drawer.DrawLine(edge.Node1.Coords, edge.Node2.Coords,
                    color);

                //redrwaing marked nodes
                var dw = ConsoleDrawer.GetDeltaWidth();
                var dh = ConsoleDrawer.GetDeltaHeight();

                drawer.WriteAt(edge.Node1.Value, (int)(edge.Node1.Coords.X * dw),
                    (int)(edge.Node1.Coords.Y * dh), ConsoleColor.Green);

                drawer.WriteAt(edge.Node2.Value, (int)(edge.Node2.Coords.X * dw),
                    (int)(edge.Node2.Coords.Y * dh), ConsoleColor.Green);
            }
        }
        public void ReplaneGraphRandom()
        {
            var rand = new Random();
            int x, y;
            foreach(var node in Nodes)
            {
                x = rand.Next(0, Constants.InitialWidth);
                y = rand.Next(0, Constants.InitialHeight);

                node.Coords = new Vector2(x,y);
            }
        }
        public void ReplaneGraphCircled()
        {
            //choose the minimum window length
            float minAspect = Math.Min(Constants.InitialWidth, Constants.InitialHeight); 
            //each edge will be 2 times smaller than the previous one.
            // So that the edges do not go beyond the window boundaries, 
            // we calculate the initial length so that the sum of the edge lengths 
            // will be equal to half of the window length
            float pow1 = (float)Math.Pow(2, Radius - 1);
            float pow2 = (float)Math.Pow(2, Radius + 1);
            float edgeLen = minAspect * pow1 / (pow2 - 2);
            //computing center 
            var center = new Vector2(Constants.InitialWidth, Constants.InitialHeight) / 2;
            CenterNode.Coords = center;
            //node and outcome edge length
            var queue = new Queue<Tuple<Node, float>>();
            var visited = new HashSet<Node>();

            queue.Enqueue(Tuple.Create(CenterNode, edgeLen));

            //forcing bfs algoritm to evaluate positions of nodes
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();

                if(visited.Contains(current.Item1)) continue;
                visited.Add(current.Item1);

                //creating pointer with outcome edge length
                var pointer = new Vector2(0, current.Item2);
                //we will rotate pointer to place outcome nodes in circle
                var deltaAng = (float)Math.PI * 2 / current.Item1.AdjasentCount;

                foreach (var adj in current.Item1.AdjasentNodes)
                {
                    adj.Coords = current.Item1.Coords + pointer;
                    queue.Enqueue(Tuple.Create(adj, current.Item2 / 2));

                    pointer = pointer.Rotate(deltaAng);
                }
            }
        }
    }
}