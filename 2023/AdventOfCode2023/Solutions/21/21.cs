using System.Dynamic;
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

    public int PartOne(int maxSteps = 64)
    {
      // Map from position to actual BFS distance
      Dictionary<(int, int), int> distances = BuildDistances(start);

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

    // Same idea as part 1 but the grid repeats forever
    // This didn't work. It's simply too large a search space for brute force.
    public int PartTwo(long maxSteps = 26501365)
    {
      Queue<((int, int) pos, int dist)> queue = new Queue<((int, int), int)>();
      queue.Enqueue((start, 0));
      // Map from position to actual BFS distance
      Dictionary<(int, int), int> distances = BuildDistances(start);
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
          // If the space contains a rock, skip it
          (int, int) rockCoordinates = CheckOriginalGrid(neighbor.Item1, neighbor.Item2);
          if (rockLocations.Contains(rockCoordinates))
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

    // Use this function to check for rocks and wraparound when calculating neighbors, instead of doing it in the main BFS loop
    private (int, int) CheckOriginalGrid(int x, int y){
      // Get original grid dimensions
      int width = strings[0].Length;
      int height = strings.Count;

      // If coordinates are out of bounds, add or subtract width/height to wrap around
      // Repeat until coordinates are within bounds
      while (x < 0) x += width;
      while (x >= width) x -= width;
      while (y < 0) y += height;
      while (y >= height) y -= height;

      // Return the wrapped coordinates
      return (x, y);
    }

    // Solution based off of this explanation: https://github.com/villuna/aoc23/wiki/A-Geometric-solution-to-advent-of-code-2023,-day-21
    // In short, there's a hidden path along the x and y axes with no rocks, so we can move directly to the farthest points
    // Then we just need to know if a repetition of the grid is an odd or even variant to know how many points we can reach in it
    // It's unfortunate that this hack isn't showcased in the example input given... so this solution doesn't seem to work for sample input. 
    public long PartTwov2(long maxSteps = 26501365)
    {
      // First, create a map of each reachable point in main grid from the Start and its distance from the Start
      Dictionary<(int, int), int> mainGridDistances = BuildDistances(start);
      // 26501365 steps is the steps from start (centre) to the edge, then the width of the grid times some unknown number.
      // For the sample input, I couldn't make this work, but for the real input:
      // 26501365 = 65 + (131 * 202300) where 65 is the distance from start to edge and 131 is the width of a single cell.
      // Let's call this 202300 our n. It's n number of full cells we can move through on the x or y axis but only in a single direction!
      // It does not include the starting cell.
      long stepsToEdge = (strings[0].Length - 1) / 2; // From centre to edge.
      long n = (maxSteps - stepsToEdge) / strings[0].Length;
      // Now some cells will be even variants, some will be odd, depending on how far they are from the centre cell. 
      // Centre cell will be odd, because the step number is odd. Then each layer (ring?) of cells around it will alternate from even/odd.
      // The post above points out that each layer is bigger than the last, growing to the next perfect square number. e.g. 1, 4, 9, 16 ...
      // So considering n, then the even variants are n^2, and the odd ones are (n+1)^2 because of the starting cell.
      long evenCells = (long)Math.Pow(n, 2);
      long oddCells = (long)Math.Pow(n + 1, 2);
      // So we can calculate the reachable cells in these areas too.
      long pointsInEvenCells = evenCells * mainGridDistances.Count(kvp => kvp.Value % 2 == 0);
      long pointsInOddCells = oddCells * mainGridDistances.Count(kvp => kvp.Value % 2 == 1);
      // But there are little corner bits between the outside cells. Fortunately, they all come in nice full numbers.
      // We'll have to subract the corners of the outermost odd layer, but we fill in the corners of even layers that we didn't completely fill.
      // Interstingly, if you add all the corners together, we are subtracting (n + 1) cells-worth of corners from the odd layers, but adding n cells-worth of corners to the even layers.
      // The corners are easily defined by the areas outside the inital 65 steps, which only reached to the straight edges of the starting cell.
      // So we just check which points are outside of that area to define the points in the corners.
      long oddCellsToSubtract = n + 1;
      long evenCellsToAdd = n;
      long pointsInOddCorners = mainGridDistances.Count(kvp => kvp.Value > stepsToEdge && kvp.Value % 2 == 1);
      long pointsInEvenCorners = mainGridDistances.Count(kvp => kvp.Value > stepsToEdge && kvp.Value % 2 == 0);

      long totalPointsRemoved = pointsInOddCorners * oddCellsToSubtract;
      long totalPointsAdded = pointsInEvenCorners * evenCellsToAdd;
      // Then the final number of points should be all the points defined in the inital cells reached, plus the points in the even corners, minus the points in the odd ones.
      long reachablePoints = pointsInEvenCells + pointsInOddCells + totalPointsAdded - totalPointsRemoved;
      return reachablePoints;
    }

    // BFS tracking actual step distance per cell
    private Dictionary<(int, int), int> BuildDistances((int, int) start)
    {
      Dictionary<(int, int), int> distanceMap = new Dictionary<(int, int), int>();
      Queue<((int, int) pos, int dist)> queue = new Queue<((int, int), int)>();
      queue.Enqueue((start, 0));  
      distanceMap[start] = 0;
      while (queue.Count > 0)
      {
        var (current, dist) = queue.Dequeue();

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
          if (!distanceMap.ContainsKey(neighbor))
          {
            distanceMap[neighbor] = dist + 1;
            queue.Enqueue((neighbor, dist + 1));
          }
        }
      }
      return distanceMap;
    }
  }
}


