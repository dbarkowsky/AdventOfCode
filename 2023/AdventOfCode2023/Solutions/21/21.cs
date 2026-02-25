using Tools;

namespace Solutions
{
  public class Day21
  {
    List<string> strings = new List<string>();
    (int, int) start = (0, 0);
    // Set to track locations of rocks
    HashSet<(int, int)> rockLocations = new HashSet<(int, int)>();


    public Day21(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      // Parse input to populate rock locations
      for (int y = 0; y < strings.Count; y++)
      {
        for (int x = 0; x < strings[y].Length; x++)
        {
          if (strings[y][x] == '#')
          {
            rockLocations.Add((x, y));
          }
          else if (strings[y][x] == 'S')
          {
            start = (x, y);
          }
        }
      }
    }

    public int PartOne(int maxSteps = 6)
    {
      // BFS tracking actual step distance per cell
      Queue<((int, int) pos, int dist)> queue = new Queue<((int, int), int)>();
      queue.Enqueue((start, 0));
      // Map from position to actual BFS distance
      Dictionary<(int, int), int> distances = new Dictionary<(int, int), int>();
      distances[start] = 0;

      while (queue.Count > 0)
      {
        var (current, dist) = queue.Dequeue();

        if (dist >= maxSteps) continue;

        var neighbors = new List<(int, int)>
        {
          (current.Item1, current.Item2 - 1), // Up
          (current.Item1, current.Item2 + 1), // Down
          (current.Item1 - 1, current.Item2), // Left
          (current.Item1 + 1, current.Item2)  // Right
        };

        foreach (var neighbor in neighbors)
        {
          // If neighbor is out of bounds, skip it
          if (neighbor.Item1 < 0 || neighbor.Item1 >= strings[0].Length || neighbor.Item2 < 0 || neighbor.Item2 >= strings.Count)
            continue;
          // If the space contains a rock, skip it
          if (rockLocations.Contains(neighbor))
            continue;
          // Only visit each cell once (BFS guarantees shortest path)
          if (!distances.ContainsKey(neighbor))
          {
            distances[neighbor] = dist + 1;
            queue.Enqueue((neighbor, dist + 1));
          }
        }
      }

      // A plot is reachable in exactly maxSteps if its shortest path distance
      // has the same parity as maxSteps (we can always "waste" 2 steps by stepping back and forth)
      return distances.Count(kvp => kvp.Value % 2 == maxSteps % 2);
    }

    public void PrintGrid(HashSet<(int, int)> visited)
    {
      for (int y = 0; y < strings.Count; y++)
      {
        for (int x = 0; x < strings[y].Length; x++)
        {
          if (rockLocations.Contains((x, y)))
            Console.Write('#');
          else if (visited.Contains((x, y)))
            Console.Write('O');
          else
            Console.Write('.');
        }
        Console.WriteLine();
      }
    }

    public int PartTwo()
    {
      return -1;
    }
  }
}


