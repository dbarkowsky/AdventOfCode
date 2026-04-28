using Tools;

namespace Solutions
{
  public class Day25
  {
    List<string> strings = new List<string>();
    Dictionary<string, List<string>> nodes = new Dictionary<string, List<string>>();
    List<string> nodeKeys = new List<string>();
    private int currentNodeKeyIndex = 0;

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
      nodeKeys.AddRange(nodes.Keys);
    }

    private class Bridge
    {
      public string from { get; }
      public string to { get; }
      public Bridge(string from, string to)
      {
        this.from = from;
        this.to = to;
      }

      public override string ToString()
      {
        return from + ":" + to;
      }
    }

    // Had an original attempt where I tried to see what nodes possibly connected to each other but didn't share other neighbours
    // That surprisingly gets the correct edges, but it also falsely identifies other edges.
    // Luckily, I did the graph theory course since then and learned this: https://github.com/dbarkowsky/GraphTheoryAlgorithms/blob/main/3_Classic_Algorithms/findBridgesAndArticulationPoints.ts
    private Dictionary<string, int> lowLink = new();
    private Dictionary<string, int> ids = new();
    private Dictionary<string, bool> visited = new();
    private List<Bridge> bridges = new();
    public int PartOne()
    {
      // Make sure there are default values for the visited and lowlink lookups.
      foreach (string key in nodeKeys)
      {
        visited[key] = false;
      }
      
      // Then run dfs for bridge detection
      foreach(string key in nodeKeys)
      {
        if (!visited[key])
        {
          DFS(key, "");
        }
      }
      Console.WriteLine(string.Join(", ", bridges));
      return -1;
    }

    private void DFS(string at, string parent)
    {
      visited[at] = true;
      // Lowlink value initialized to current key index
      lowLink[at] = currentNodeKeyIndex;
      ids[at] = currentNodeKeyIndex;
      currentNodeKeyIndex++;

      // Check each neighbour
      nodes[at].ForEach((string neighbour) =>
      {
        // skip if parent
        if (parent.Equals(neighbour)) return;
        if (!visited[neighbour])
        {
          // Recursive dfs
          DFS(neighbour, at);
          // At this point, we've gone all the way to the end and are starting to recurse back up.
          // Set the lowlink value
          lowLink[at] = Math.Min(lowLink[at], lowLink[neighbour]);
          // This is where things are weird. We need to check their indexes. This would have been an id in another system.
          if (ids[at] < lowLink[neighbour])
          {
            bridges.Add(new Bridge(at, neighbour));
          }
        } else
        {
          // It's been visited before. Update the lowlink value
          lowLink[at] = Math.Min(lowLink[at], ids[neighbour]);
        }
      });
    }

    // Part 2 is a freebie if you've already got all other puzzles solved.
    public int PartTwo()
    {
      return -1;
    }
  }
}


