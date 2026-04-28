using Tools;

namespace Solutions
{
  public class Day25
  {
    List<string> strings = new List<string>();
    Dictionary<string, List<string>> nodes = new Dictionary<string, List<string>>();
    Dictionary<string, List<FlowEdge>> flowGraph = new();
    HashSet<string> visited = new();


    public Day25(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      // Ensure each line has an entry with a new list
      foreach (string node in strings)
      {
        nodes[node.Split(": ").First()] = new List<string>();
        // Not all nodes are left side, some only in right, must add those too
        foreach (string subnode in node.Split(": ").Last().Split(" "))
        {
          nodes[subnode] = new List<string>();
        }
      }

      // Now go through and connect those nodes
      foreach (string node in strings)
      {
        string nodeName = node.Split(": ").First();
        string[] connections = node.Split(": ").Last().Split(' ');
        foreach (string connection in connections)
        {
          // Add connection to the node
          nodes[nodeName].Add(connection);
          // Add this node to the connection's list as well
          nodes[connection].Add(nodeName);
        }
      }

      BuildFlowGraph();
    }

    private class FlowEdge
    {
      public string from { get; }
      public string to { get; }
      public int capacity { get; }
      public int flow { get; set; }
      public FlowEdge? residual { get; set; }

      public FlowEdge(string from, string to, int capacity)
      {
        this.from = from;
        this.to = to;
        this.capacity = capacity;
        this.flow = 0;
      }

      public int RemainingCapacity() => capacity - flow;

      public void AugmentFlow(int amount)
      {
        flow += amount;
        residual!.flow -= amount;
      }
    }

    // Had an original attempt where I tried to see what nodes possibly connected to each other but didn't share other neighbours
    // That surprisingly gets the correct edges, but it also falsely identifies other edges.
    // Then I thought this would work: https://github.com/dbarkowsky/GraphTheoryAlgorithms/blob/main/3_Classic_Algorithms/findBridgesAndArticulationPoints.ts
    // But it looks for points where a single cut would separate the graph. The question needs a minimum of 3 (maybe exactly 3).
    // So new approach: Get the flow of the entire graph. Find the edges that would carry the max flow together. Those must be the bridges.
    // This is also the Ford-Fulkerson method: https://github.com/dbarkowsky/GraphTheoryAlgorithms/blob/main/4_Network_Flow/fordFulkerson.ts
    // One thing that helps: there can't be any leaves in this graph by puzzle design. Otherwise, it would be too easy to cut those edges to separate the graph.
    public int PartOne()
    {
      // We can start on any node
      string source = nodes.Keys.First();
      // We know the max flow we need is 3, because that's how many the cut requirement is.
      int targetBridges = 3;
      // So we try different sink options until it reveals itself
      string sink = "";
      // First time seeing this skip syntax! How often is this useful?
      foreach (string potentialSink in nodes.Keys.Skip(1))
      {
        // Start with a reset flow each time
        foreach (List<FlowEdge> edgeList in flowGraph.Values)
        {
          foreach (FlowEdge edge in edgeList)
          {
            edge.flow = 0;
          }
        }

        // then see if it matches our goal
        if (MaxFlow(source, potentialSink) == targetBridges)
        {
          sink = potentialSink;
          break;
        }
      }

      // Now we have our sink figured out, we must BFS only the edges that still have capacity.
      HashSet<string> visited = new();
      Queue<string> queue = new();
      queue.Enqueue(source);
      visited.Add(source);

      // Continue until queue is empty, doing BFS. We'll record which nodes we visit along the way.
      // This will be all the nodes which connecting edges with capacity...
      while (queue.Count > 0)
      {
        string current = queue.Dequeue();
        foreach (FlowEdge edge in flowGraph[current])
        {
          if (edge.RemainingCapacity() > 0 && !visited.Contains(edge.to))
          {
            visited.Add(edge.to);
            queue.Enqueue(edge.to);
          }
        }
      }

      // But it's the sizes we want
      int groupASize = visited.Count;
      int groupBSize = nodes.Count - groupASize;

      return groupASize * groupBSize;
    }

    // Part 2 is a freebie if you've already got all other puzzles solved.
    public int PartTwo()
    {
      return -1;
    }

    private void BuildFlowGraph()
    {
      // New list of edges for every node
      foreach (string key in nodes.Keys)
      {
        flowGraph[key] = new();
      }
      // Undirected edges, so we'll add a forward and residual edge
      foreach (string from in nodes.Keys)
      {
        foreach (string to in nodes[from])
        {
          // Add the edges
          FlowEdge forward = new FlowEdge(from, to, 1); // All forward edges have capacity of 1.
          FlowEdge backward = new FlowEdge(to, from, 0); // But residuals are 0 at first...

          forward.residual = backward;
          backward.residual = forward;

          flowGraph[from].Add(forward);
          flowGraph[to].Add(backward);
        }
      }
    }

    private int DFS(string at, string sink, int flow)
    {
      // We reach the sink? It's over
      if (at == sink) return flow;

      visited.Add(at);

      foreach (FlowEdge edge in flowGraph[at])
      {
        if (edge.RemainingCapacity() > 0 && !visited.Contains(edge.to))
        {
          int bottleneck = Math.Min(flow, edge.RemainingCapacity()); // Keep either previous bottleneck or new smaller capacity.
          int resultFlow = DFS(edge.to, sink, bottleneck); // Continue DFS to find the sink

          // At this point, we must have made it to the sink.
          if (resultFlow > 0)
          {
            edge.AugmentFlow(resultFlow);
            return resultFlow;
          }
        }
      }
      return 0; // Did not reach sink.
    }

    private int MaxFlow(string source, string sink)
    {
      int total = 0;
      int flow;
      do
      {
        visited.Clear();
        flow = DFS(source, sink, int.MaxValue);
        total += flow;
      } while (flow > 0);
      return total;
    }
  }
}


