using Tools;

namespace Solutions
{
  public class Day25
  {
    List<string> strings = new List<string>();
    Dictionary<string, List<string>> nodes = new Dictionary<string, List<string>>();


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
    }

    public int PartOne()
    {
      // Need to find which nodes connect to each other but have no other shared connections
      HashSet<(string, string)> connectionPairs = new HashSet<(string, string)>();
      //// So for each node
      foreach (string key in nodes.Keys)
      {
        // Check each connection
        foreach (string connectionKey in nodes[key])
        {
          // If the current node and the connected node don't share any connections
          if (NumberOfMatchedConnections(nodes[key], nodes[connectionKey]) == 0)
          {
            // Need to sort these so they're always inserted the same. 
            // AKA (a, b) always, never (b, a)
            List<string> pair = new List<string> { key, connectionKey };
            pair.Sort();
            connectionPairs.Add((pair.First(), pair.Last()));
          }
        }
      }
      foreach ((string, string) pair in connectionPairs)
      {
        Console.WriteLine($"{pair.Item1}, {pair.Item2}");
      }
      // With that list, remember one from each side of a pair for later
      // But cut the connections by removing them from each other's lists

      // Then, step through all connected nodes on both sides to get the counts
      // Multiply those counts
      return -1;
    }

    public int PartTwo()
    {
      return -1;
    }

    private int NumberOfMatchedConnections(List<string> l1, List<string> l2)
    {
      int numOfMatches = 0;
      foreach (string node in l1)
      {
        if (l2.Contains(node))
        {
          numOfMatches++;
        }
      }
      return numOfMatches;
    }
  }
}


